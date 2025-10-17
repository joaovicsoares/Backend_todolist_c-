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
public class TaskController : ControllerBase
{
    private readonly AppDbContext _context;
    public TaskController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public IActionResult GetTarefas(int id)
    {
        try
        {
            var tarefas = _context.Tarefas.Where(t => t.IdLista == id).ToList();
            return Ok(tarefas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar tarefas", error = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult CreateTarefa([FromBody] int idlista, Tarefa novaTarefa)
    {
        if (string.IsNullOrEmpty(novaTarefa.Titulo))
        {
            return BadRequest("O titulo da tarefa não pode estar vazio");
        }
        try
        {
            var tarefa = new Tarefa { Titulo = novaTarefa.Titulo, IdLista = idlista };
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTarefas), new { id = tarefa.Id }, tarefa);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao adicionar tarefa", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTarefa(int id)
    {
        try
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa == null)
            {
                return NotFound(new { message = "Tarefa não encontrada" });
            }
            _context.Tarefas.Remove(tarefa);
            _context.SaveChanges();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao deletar tarefa", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTarefa(int id, [FromBody] Tarefa novosDados)
    {
        try
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa == null)
            {
                return NotFound(new { message = "Tarefa não encontrada" });
            }

            tarefa.Concluida = novosDados.Concluida;
            _context.SaveChanges();
            return Ok(tarefa);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao atualizar Tarefa", error = ex.Message });
        }
    }

}