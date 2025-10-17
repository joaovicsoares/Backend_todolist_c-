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
public class ListController : ControllerBase
{
    private readonly AppDbContext _context;

    public ListController(AppDbContext context)
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

    [HttpGet]
    public IActionResult GetListas()
    {
        try
        {
            var usuarioId = GetUsuarioId();
            var listas = _context.Listas.Where(l => l.ListaUsuarios.Any(lu => lu.IdUsuario == usuarioId)).ToList();
            return Ok(listas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar listas de tarefas", error = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult CreateLista([FromBody] Lista novaLista)
    {
        if (string.IsNullOrEmpty(novaLista.Nome))
        {
            return BadRequest(new { message = "O nome da lista é obrigatorio" });
        }
        try
        {
            var usuarioId = GetUsuarioId();
            var lista = new Lista { Nome = novaLista.Nome };
            _context.Listas.Add(lista);
            _context.SaveChanges();

            var associacao = new ListaUsuario
            {
                IdUsuario = usuarioId,
                IdLista = lista.Id
            };
            _context.ListaUsuarios.Add(associacao);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetListas), new { id = lista.Id }, lista);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao criar lista", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteLista(int id)
    {
        try
        {
            if (!AcessoLista(id))
            {
                return Forbid("Voce não tem permissão para deletar essa lista");
            }
            var lista = _context.Listas.Find(id);
            if (lista == null)
            {
                return NotFound(new { message = "Lista não encontrada" });
            }
            _context.Listas.Remove(lista);
            _context.SaveChanges();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao deletar lista", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateLista(int id, [FromBody]  Lista dadosUpdate)
    {
        if (string.IsNullOrEmpty(dadosUpdate.Nome))
        {
            return BadRequest(new { message = "O nome da lista é obrigatorio" });
        }
        try
        {
            if (!AcessoLista(id))
            {
                return Forbid("Voce não tem permissão para atualizar esta lista");
            }
            var lista = _context.Listas.Find(id);
            if (lista == null)
            {
                return NotFound(new { message = "Lista não encontrada" });
            }
            lista.Nome = dadosUpdate.Nome;
            _context.SaveChanges();
            return Ok(lista);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atualizar lista", error = ex.Message});
        }
    }
}
