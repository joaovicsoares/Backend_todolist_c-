namespace Servidor.Models;

public class Lista
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;


    public ICollection<ListaUsuario> ListaUsuarios { get; set; }
}