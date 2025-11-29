using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Web.Interfaces;
using Firmeza.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Firmeza.Api.Services;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public SalesController(ISaleRepository saleRepository, IProductRepository productRepository, IUserRepository userRepository, IEmailService emailService, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _emailService = emailService;
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

    [HttpGet("user/{email}")]
    public async Task<IActionResult> GetByUserEmail(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var sales = await _saleRepository.GetAllAsync();
        var userSales = sales.Where(s => s.UserId == user.Id).ToList();
        var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(userSales);
        return Ok(saleDtos);
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

            // Validate User - UserName from frontend is actually the email
            var user = await _userRepository.GetByEmailAsync(model.UserName);
            if (user == null)
            {
                return BadRequest(new { message = $"User with email '{model.UserName}' not found." });
            }

            string userId = user.Id;

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
                UserId = userId,
                Date = DateTime.UtcNow,
                Total = total,
                Vat = total * 0.19m, // 19% VAT
                SaleDetails = saleDetails
            };

            await _saleRepository.AddAsync(sale);

            // Send Email Confirmation
            try
            {
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    // Prepare product items for email
                    var emailItems = new List<(string productName, int quantity, decimal unitPrice, decimal subtotal)>();
                    foreach (var detail in saleDetails)
                    {
                        var product = await _productRepository.GetById(detail.ProductId);
                        emailItems.Add((
                            product?.Name ?? "Producto",
                            detail.Quantity,
                            detail.UnitPrice,
                            detail.Subtotal
                        ));
                    }

                    var subject = $"✅ Confirmación de Compra - Orden #{sale.Id}";
                    var body = _emailService.GeneratePurchaseReceipt(
                        user.FullName,
                        sale.Id,
                        sale.Date,
                        sale.Total,
                        sale.Vat,
                        emailItems
                    );

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }
            }
            catch (Exception emailEx)
            {
                // Log email error but don't fail the request
                Console.WriteLine($"⚠️  Error sending email: {emailEx.Message}");
            }

            var saleDto = _mapper.Map<SaleDto>(sale);
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, saleDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating sale", error = ex.Message });
        }
    }
}
