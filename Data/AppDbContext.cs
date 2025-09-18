using Microsoft.EntityFrameworkCore;

namespace ProjetoPCRH.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Projeto> Projetos { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Faturacao> Faturas { get; set; }
       

        public DbSet<Relatorio> Relatorios { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<FuncionarioProjeto> FuncionarioProjetos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add your model configurations here
            modelBuilder.Entity<Contrato>()           
              .HasOne(c => c.Projeto)
             .WithMany()
             .HasForeignKey(c => c.ProjetoId)
             .OnDelete(DeleteBehavior.Restrict);

          

            modelBuilder.Entity<FuncionarioProjeto>()
                .HasKey(fp => new { fp.FuncionarioId, fp.ProjetoId });

            modelBuilder.Entity<FuncionarioProjeto>()
                .HasOne(fp => fp.Funcionario)
                .WithMany(f => f.FuncionarioProjetos)
                .HasForeignKey(fp => fp.FuncionarioId);

            modelBuilder.Entity<FuncionarioProjeto>()
                .HasOne(fp => fp.Projeto)
                .WithMany(p => p.FuncionarioProjetos)
                .HasForeignKey(fp => fp.ProjetoId);
        }
    
    }
}
