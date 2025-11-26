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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            decimal total = 0;
            var saleDetails = new List<SaleDetail>();

            // Validate User
            if (string.IsNullOrEmpty(model.UserId) && string.IsNullOrEmpty(model.UserName))
            {
                return BadRequest(new { message = "Either UserId or UserName must be provided." });
            }

            string userId = model.UserId;
            User? user = null;

            if (!string.IsNullOrEmpty(userId))
            {
                user = await _userRepository.GetByIdAsync(userId);
            }
            else if (!string.IsNullOrEmpty(model.UserName))
            {
                user = await _userRepository.GetByNameAsync(model.UserName);
                if (user != null)
                {
                    userId = user.Id;
                }
            }

            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

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
                // User is already retrieved above
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var subject = $"Confirmación de Compra - Orden #{sale.Id}";
                    var body = $@"
                        <h1>Gracias por tu compra, {user.FullName}!</h1>
                        <p>Tu orden <strong>#{sale.Id}</strong> ha sido registrada exitosamente.</p>
                        <p><strong>Fecha:</strong> {sale.Date}</p>
                        <p><strong>Total:</strong> {sale.Total:C}</p>
                        <h3>Detalles:</h3>
                        <ul>
                            {string.Join("", saleDetails.Select(d => $"<li>Producto ID: {d.ProductId} - Cantidad: {d.Quantity} - Subtotal: {d.Subtotal:C}</li>"))}
                        </ul>
                        <p>Gracias por preferirnos.</p>
                    ";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }
            }
            catch (Exception emailEx)
            {
                // Log email error but don't fail the request
                Console.WriteLine($"Error sending email: {emailEx.Message}");
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
