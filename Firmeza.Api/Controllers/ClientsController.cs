using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ClientsController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public ClientsController(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allUsers = _userManager.Users.ToList();
        var clients = new List<User>();

        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Cliente"))
            {
                clients.Add(user);
            }
        }

        var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);
        return Ok(clientDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Client not found" });

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("Cliente"))
            return NotFound(new { message = "User is not a client" });

        var clientDto = _mapper.Map<ClientDto>(user);
        return Ok(clientDto);
    }

    [HttpPost("{id}/assign-role")]
    public async Task<IActionResult> AssignClientRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Cliente"))
            return BadRequest(new { message = "User already has Cliente role" });

        var result = await _userManager.AddToRoleAsync(user, "Cliente");
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = $"Cliente role assigned to {user.FullName}" });
    }
}
