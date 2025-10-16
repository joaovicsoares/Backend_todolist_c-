using Microsoft.EntityFrameworkCore;
using Servidor.Models;

namespace Servidor.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<Lista> Listas { get; set; }
    public DbSet<ListaUsuario> ListaUsuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios"); // tabela
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Senha).HasColumnName("senha");
        });

        modelBuilder.Entity<Tarefa>(entity =>
        {
            entity.ToTable("tarefas");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Titulo).HasColumnName("titulo");
            entity.Property(e => e.Concluida).HasColumnName("concluida");
        });

        modelBuilder.Entity<Lista>(entity =>
        {
            entity.ToTable("listas");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nome).HasColumnName("nome");
        });

        modelBuilder.Entity<ListaUsuario>(entity =>
        {
            entity.ToTable("lista_usuario");
            entity.Property(e => e.IdUsuario).HasColumnName("idusuario");
            entity.Property(e => e.IdLista).HasColumnName("idlista");
        });
    }
}
