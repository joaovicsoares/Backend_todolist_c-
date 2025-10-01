namespace Servidor.Models;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public bool Concluida { get; set; } = false;
}