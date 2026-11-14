using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoAPI.Data;
using ProyectoAPI.DTOs;
using ProyectoAPI.Models;

namespace ProyectoAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Select(u => new
            {
                u.Id,
                u.Nombre,
                u.Correo
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);

        if (usuario == null)
        {
            return NotFound(new
            {
                mensaje = "Usuario no encontrado"
            });
        }

        return Ok(new
        {
            usuario.Id,
            usuario.Nombre,
            usuario.Correo
        });
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CrearUsuario(UsuarioCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) ||
            string.IsNullOrWhiteSpace(dto.Correo) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new
            {
                mensaje = "Todos los campos son obligatorios"
            });
        }

        var correoExiste = await _context.Usuarios
            .AnyAsync(u => u.Correo == dto.Correo);

        if (correoExiste)
        {
            return BadRequest(new
            {
                mensaje = "El correo ya está registrado"
            });
        }

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Correo = dto.Correo
        };

        usuario.PasswordHash = _passwordHasher.HashPassword(
            usuario,
            dto.Password
        );

        _context.Usuarios.Add(usuario);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(ObtenerUsuario),
            new { id = usuario.Id },
            new
            {
                usuario.Id,
                usuario.Nombre,
                usuario.Correo
            }
        );
    }
}