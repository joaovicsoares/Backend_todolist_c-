namespace Servidor.Models;

public class ListaUsuario
{
    public int IdUsuario { get; set; }
    public int IdLista { get; set; }

    public Lista Lista { get; set; }
    public Usuario Usuario { get; set; }
}