using Microsoft.AspNetCore.Mvc;
using Servidor.Data;
using Servidor.Models;
using Servidor.Dtos.Auth;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace Servidor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("signup")]
    public IActionResult Signup([FromBody] SignupDto usuarioDto)
    {
        if (string.IsNullOrEmpty(usuarioDto.Nome) || string.IsNullOrEmpty(usuarioDto.Email) || string.IsNullOrEmpty(usuarioDto.Senha))
            return BadRequest(new { message = "Todos os campos são obrigatórios." });

            var usuario = new Usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha)
            };
            
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        return Ok(new { message = "Usuário criado com sucesso!", usuario.Id, usuario.Email });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto usuarioDto)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioDto.Email);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(usuarioDto.Senha, usuario.Senha))
            return BadRequest(new { message = "Email ou senha inválidos." });

        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var token = new JwtSecurityToken(
            claims: new[] { new Claim("id", usuario.Id.ToString()), new Claim("email", usuario.Email) },
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }
}