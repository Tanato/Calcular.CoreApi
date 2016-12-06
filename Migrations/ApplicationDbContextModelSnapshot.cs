using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Calcular.CoreApi.Models;

namespace Calcular.CoreApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Atividade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AtividadeOrigemId");

                    b.Property<DateTime?>("Entrega");

                    b.Property<int>("EtapaAtividade");

                    b.Property<string>("Nome");

                    b.Property<string>("Observacao");

                    b.Property<string>("ObservacaoRevisor");

                    b.Property<string>("ResponsavelId");

                    b.Property<int>("ServicoId");

                    b.Property<decimal?>("Tempo");

                    b.Property<int>("TipoAtividadeId");

                    b.Property<int?>("TipoExecucao");

                    b.Property<int?>("TipoImpressao");

                    b.HasKey("Id");

                    b.HasIndex("AtividadeOrigemId");

                    b.HasIndex("ResponsavelId");

                    b.HasIndex("ServicoId");

                    b.HasIndex("TipoAtividadeId");

                    b.ToTable("Atividades");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Cliente", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Celular");

                    b.Property<string>("Celular2");

                    b.Property<int?>("ComoChegou");

                    b.Property<string>("ComoChegouDetalhe");

                    b.Property<string>("Email");

                    b.Property<string>("Empresa");

                    b.Property<string>("Endereco");

                    b.Property<decimal?>("Honorarios");

                    b.Property<DateTime?>("Nascimento");

                    b.Property<string>("Nome");

                    b.Property<string>("Observacao");

                    b.Property<int?>("Perfil");

                    b.Property<string>("Telefone");

                    b.Property<string>("Telefone2");

                    b.HasKey("Id");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Cobranca", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Contato");

                    b.Property<DateTime>("DataCobranca");

                    b.Property<string>("Observacao");

                    b.Property<DateTime?>("PrevisaoPagamento");

                    b.Property<int>("ProcessoId");

                    b.Property<string>("UsuarioId");

                    b.Property<decimal>("ValorPendente");

                    b.HasKey("Id");

                    b.HasIndex("ProcessoId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Cobrancas");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Honorario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Data");

                    b.Property<string>("NotaFiscal");

                    b.Property<string>("Observacao");

                    b.Property<DateTime?>("Prazo");

                    b.Property<int>("ProcessoId");

                    b.Property<int>("Registro");

                    b.Property<int>("TipoPagamento");

                    b.Property<decimal>("Valor");

                    b.HasKey("Id");

                    b.HasIndex("ProcessoId");

                    b.ToTable("Honorarios");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Processo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdvogadoId");

                    b.Property<string>("Autor");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Indicacao");

                    b.Property<int>("Local");

                    b.Property<string>("Numero");

                    b.Property<int>("NumeroAutores");

                    b.Property<int>("Parte");

                    b.Property<string>("Perito");

                    b.Property<string>("Reu");

                    b.Property<string>("Vara");

                    b.HasKey("Id");

                    b.HasIndex("AdvogadoId");

                    b.ToTable("Processos");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.ProcessoDetalhe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Data");

                    b.Property<string>("Descricao");

                    b.Property<int>("ProcessoId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ProcessoId");

                    b.HasIndex("UserId");

                    b.ToTable("ProcessoDetalhes");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Proposta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Celular");

                    b.Property<int>("ComoChegou");

                    b.Property<string>("ComoChegouDetalhe");

                    b.Property<string>("Contato");

                    b.Property<int>("ContatoId");

                    b.Property<DateTime?>("DataProposta");

                    b.Property<string>("Email");

                    b.Property<bool>("Fechado");

                    b.Property<int>("Honorario");

                    b.Property<int>("Local");

                    b.Property<int>("Motivo");

                    b.Property<string>("MovitoDetalhe");

                    b.Property<string>("Numero");

                    b.Property<string>("Observacao");

                    b.Property<int?>("ProcessoId");

                    b.Property<string>("Telefone");

                    b.Property<string>("UsuarioId");

                    b.HasKey("Id");

                    b.HasIndex("ProcessoId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Propostas");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Servico", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Entrada");

                    b.Property<DateTime?>("Prazo");

                    b.Property<int>("ProcessoId");

                    b.Property<DateTime?>("Saida");

                    b.Property<int>("Status");

                    b.Property<string>("Volumes");

                    b.HasKey("Id");

                    b.HasIndex("ProcessoId");

                    b.ToTable("Servicos");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.TipoAtividade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Nome")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("Nome");

                    b.ToTable("TipoAtividades");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Description");

                    b.Property<int>("Type");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.User", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("Inativo");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("Password");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Atividade", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.Business.Atividade", "AtividadeOrigem")
                        .WithMany()
                        .HasForeignKey("AtividadeOrigemId");

                    b.HasOne("Calcular.CoreApi.Models.User", "Responsavel")
                        .WithMany("Atividades")
                        .HasForeignKey("ResponsavelId");

                    b.HasOne("Calcular.CoreApi.Models.Business.Servico", "Servico")
                        .WithMany("Atividades")
                        .HasForeignKey("ServicoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Calcular.CoreApi.Models.Business.TipoAtividade", "TipoAtividade")
                        .WithMany()
                        .HasForeignKey("TipoAtividadeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Cobranca", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.Business.Processo", "Processo")
                        .WithMany("Cobrancas")
                        .HasForeignKey("ProcessoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Calcular.CoreApi.Models.User", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Honorario", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.Business.Processo", "Processo")
                        .WithMany("Honorarios")
                        .HasForeignKey("ProcessoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Processo", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.Business.Cliente", "Advogado")
                        .WithMany("Processos")
                        .HasForeignKey("AdvogadoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.ProcessoDetalhe", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.Business.Processo", "Processo")
                        .WithMany("ProcessoDetalhes")
                        .HasForeignKey("ProcessoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Calcular.CoreApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Proposta", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.Business.Processo", "Processo")
                        .WithMany("Propostas")
                        .HasForeignKey("ProcessoId");

                    b.HasOne("Calcular.CoreApi.Models.User", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId");
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Business.Servico", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.Business.Processo", "Processo")
                        .WithMany("Servicos")
                        .HasForeignKey("ProcessoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Calcular.CoreApi.Models.Log", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.User", "User")
                        .WithMany("UserLogs")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Calcular.CoreApi.Models.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Calcular.CoreApi.Models.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
