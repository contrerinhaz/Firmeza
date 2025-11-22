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
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] string? category = null)
    {
        var products = await _productRepository.GetAll();

        // Apply filters
        if (!string.IsNullOrEmpty(search))
        {
            products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category != null && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(productDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productRepository.GetById(id);
        if (product == null)
            return NotFound(new { message = "Product not found" });

        var productDto = _mapper.Map<ProductDto>(product);
        return Ok(productDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = _mapper.Map<Product>(model);
        await _productRepository.Create(product);

        var productDto = _mapper.Map<ProductDto>(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, productDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productRepository.GetById(id);
        if (product == null)
            return NotFound(new { message = "Product not found" });

        product.Name = model.Name;
        product.Price = model.Price;
        product.Quantity = model.Quantity;
        product.Category = model.Category;

        await _productRepository.Update(product);

        var productDto = _mapper.Map<ProductDto>(product);
        return Ok(productDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetById(id);
        if (product == null)
            return NotFound(new { message = "Product not found" });

        await _productRepository.Delete(id);
        return Ok(new { message = "Product deleted successfully" });
    }
}
