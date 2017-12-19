using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Calcular.CoreApi.Models.Business;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Calcular.CoreApi.Models
{
<<<<<<< HEAD
    public class ApplicationContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Calcular.CoreSql;Integrated Security=True");

            return new ApplicationDbContext(optionsBuilder.Options);
=======
    public class TemporaryDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext Create()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Calcular.CoreSql;Integrated Security=True");
            return new ApplicationDbContext(builder.Options);
>>>>>>> master
        }

        public ApplicationDbContext Create(DbContextFactoryOptions options)
        {
<<<<<<< HEAD
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Calcular.CoreSql;Integrated Security=True");

            return new ApplicationDbContext(optionsBuilder.Options);
=======
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Calcular.CoreSql;Integrated Security=True");
            return new ApplicationDbContext(builder.Options);
>>>>>>> master
        }
    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Atividade> Atividades { get; set; }
        public DbSet<TipoAtividade> TipoAtividades { get; set; }
        public DbSet<Processo> Processos { get; set; }
        public DbSet<FaseProcesso> FaseProcessos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<TipoServico> TipoServicos { get; set; }
        public DbSet<Honorario> Honorarios { get; set; }
        public DbSet<ProcessoDetalhe> ProcessoDetalhes { get; set; }
        public DbSet<Cobranca> Cobrancas { get; set; }
        public DbSet<Proposta> Propostas { get; set; }
        public DbSet<Comissao> Comissoes { get; set; }
        public DbSet<ComissaoFuncionarioMes> ComissoesFuncionarioMes { get; set; }
        public DbSet<ComissaoAtividade> ComissaoAtividades { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityRole>().ToTable("Roles");

            builder.Entity<TipoAtividade>().HasAlternateKey(x => x.Nome);

            builder.Entity<Processo>()
                .Property(p => p.Total)
                .HasColumnType("money")
                .HasComputedColumnSql(@"[dbo].TotalHonorarios([Id])");
        }
    }
}