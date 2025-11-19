namespace Firmeza.Web.Controllers;

using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Models.Entities;

[Authorize(Roles = "Admin")]

public class ProductsController : Controller 
{
    private readonly IProductService _productService;

        public ProductsController(IProductService productservice)
        {
            _productService = productservice;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProducts();
            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _productService.GetProductById(id.Value);
            
            if (product == null)
                return NotFound();

            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Quantity,Category")] Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            product.CreatedAt = DateTime.UtcNow;

            var created = await _productService.CreateProduct(product);
            if (created)
            {
                TempData["Success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _productService.GetProductById(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Quantity,Category,CreatedAt")] Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(product);

            try
            {
                var updated = await _productService.UpdateProduct(product);
                if (updated)
                {
                    TempData["Success"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                return View(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                    return NotFound();
                else
                    throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _productService.GetProductById(id.Value);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _productService.DeleteProduct(id);
            if (deleted)
            {
                TempData["Success"] = "Product deleted successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while deleting the product.");
            var product = await _productService.GetProductById(id);
            return View(product);
        }
        private bool ProductExists(int id)
        {
            var product = _productService.GetProductById(id).Result;
            return product != null;
        }
}

