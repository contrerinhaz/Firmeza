using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Web.Interfaces;
using Firmeza.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public SalesController(ISaleRepository saleRepository, IProductRepository productRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _saleRepository.GetAllAsync();
        var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(sales);
        return Ok(saleDtos);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _saleRepository.GetByIdAsync(id);
        if (sale == null)
            return NotFound(new { message = "Sale not found" });

        var saleDto = _mapper.Map<SaleDto>(sale);
        return Ok(saleDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            decimal total = 0;
            var saleDetails = new List<SaleDetail>();

            foreach (var detail in model.Details)
            {
                var product = await _productRepository.GetById(detail.ProductId);
                if (product == null)
                    return BadRequest(new { message = $"Product {detail.ProductId} not found" });

                if (product.Quantity < detail.Quantity)
                    return BadRequest(new { message = $"Insufficient stock for product {product.Name}" });

                var saleDetail = new SaleDetail
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    UnitPrice = product.Price
                };

                saleDetails.Add(saleDetail);
                total += saleDetail.Subtotal;

                // Update product stock
                product.Quantity -= detail.Quantity;
                await _productRepository.Update(product);
            }

            var sale = new Sale
            {
                UserId = model.UserId,
                Date = DateTime.UtcNow,
                Total = total,
                Vat = total * 0.19m, // 19% VAT
                SaleDetails = saleDetails
            };

            await _saleRepository.AddAsync(sale);

            var saleDto = _mapper.Map<SaleDto>(sale);
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, saleDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating sale", error = ex.Message });
        }
    }
}
