namespace Firmeza.Web.Controllers;

using Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Models.Entities;
using ViewModels.Users;

[Authorize(Policy = "AdminOnly")]

// Controller for managing users (Admin only)
public class UsersController : Controller
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    // GET: Users
    // Lists all registered users
    public async Task<IActionResult> Index(int pageNumber = 1)
    {
        const int pageSize = 10;
        var users = await _service.GetPagedAsync(pageNumber, pageSize);
        return View(users);
    }

    // GET: Users/Details/5
    // Shows details of a specific user
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
            return NotFound();

        var user = await _service.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    // GET: Users/Create
    // Prepares view for creating a new user
    public IActionResult Create()
    {
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Processes the creation of a new user
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName,
            DocumentNumber = model.DocumentNumber,
            Phone = model.Phone,
            RegisterDate = model.RegisterDate,
            EmailConfirmed = true
        };

        try
        {
            // Aquí usamos Identity para crear usuario + contraseña + rol
            await _service.CreateWithPasswordAsync(user, model.Password, "Client");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    // GET: Users/Edit/5
    // Prepares view for editing an existing user
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
            return NotFound();

        var user = await _service.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    // Processes the update of a user
    public async Task<IActionResult> Edit(string id, User user)
    {
        if (id != user.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(user);

        try
        {
            await _service.UpdateAsync(user);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _service.ExistsAsync(user.Id))
                return NotFound();

            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Users/Delete/5
    // Prepares view for deleting a user
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
            return NotFound();

        var user = await _service.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    // Confirms and executes the deletion of a user
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
