using Microsoft.AspNetCore.Mvc;
using Servidor.Data;
using Servidor.Models;
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
    public IActionResult Signup([FromBody] Usuario usuario)
    {
        if (string.IsNullOrEmpty(usuario.Nome) || string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.Senha))
            return BadRequest(new { message = "Todos os campos são obrigatórios." });

        usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        return Ok(new { message = "Usuário criado com sucesso!", usuario.Id, usuario.Email });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Usuario dados)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == dados.Email);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dados.Senha, usuario.Senha))
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