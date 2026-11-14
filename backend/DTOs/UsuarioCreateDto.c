namespace ProyectoAPI.DTOs;

public class UsuarioCreateDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}