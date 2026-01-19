using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Servidor.Data;
using Servidor.Models;
using Servidor.Dtos.Share;
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
    public IActionResult ShareList([FromBody] ShareListDto shareDto)
    {
        try
        {
            // ✅ Verificar se quem está compartilhando tem acesso à lista
            if (!AcessoLista(shareDto.IdLista))
            {
                return Forbid("Você não tem permissão para compartilhar esta lista");
            }

            // ✅ Buscar usuário pelo email
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == shareDto.Email);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuário com este email não encontrado" });
            }

            // ✅ Verificar se já não está compartilhado
            var jaCompartilhado = _context.ListaUsuarios.Any(lu => 
                lu.IdLista == shareDto.IdLista && lu.IdUsuario == usuario.Id);
            if (jaCompartilhado)
            {
                return Conflict(new { message = "Lista já compartilhada com este usuário" });
            }

            // ✅ Criar o compartilhamento
            var associacao = new ListaUsuario 
            { 
                IdLista = shareDto.IdLista, 
                IdUsuario = usuario.Id 
            };
            _context.ListaUsuarios.Add(associacao);
            _context.SaveChanges();

            return Ok(new { 
                message = "Lista compartilhada com sucesso", 
                usuarioEmail = usuario.Email,
                usuarioNome = usuario.Nome
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro no compartilhamento de lista", error = ex.Message });
        }
    }

}