using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Servidor.Data;
using Servidor.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Servidor.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShareListController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShareListController(AppDbContext context)
    {
        _context = context;
    }

    private int GetUsuarioId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "id");
        return int.Parse(claim.Value);
    }

    private bool AcessoLista(int idLista)
    {
        var usuarioId = GetUsuarioId();
        return _context.ListaUsuarios.Any(lu => lu.IdLista == idLista && lu.IdUsuario == usuarioId);
    }

    [HttpPost]

    public IActionResult ShareList([FromBody] ListaUsuario novoUsuario)
    {
        try
        {
            var associacao = new ListaUsuario { IdLista = novoUsuario.IdLista, IdUsuario = novoUsuario.IdUsuario };
            _context.ListaUsuarios.Add(associacao);
            _context.SaveChanges();

            return Created();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro no compartilhamento de lista", error = ex.Message });
        }
    }

}