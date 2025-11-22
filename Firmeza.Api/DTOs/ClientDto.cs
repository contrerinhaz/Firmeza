namespace Firmeza.Api.DTOs;

public class ClientDto
{
    public required string Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required string DocumentNumber { get; set; }
}
