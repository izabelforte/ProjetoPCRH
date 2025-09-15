using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoPCRH.Migrations
{
    /// <inheritdoc />
    public partial class mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nif = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteId);
                });

            migrationBuilder.CreateTable(
                name: "Funcionarios",
                columns: table => new
                {
                    FuncionarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeFuncionario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nif = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAdmissao = table.Column<DateTime>(type: "date", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionarios", x => x.FuncionarioId);
                });

            migrationBuilder.CreateTable(
                name: "Projetos",
                columns: table => new
                {
                    ProjetoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeProjeto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "date", nullable: false),
                    DataFim = table.Column<DateTime>(type: "date", nullable: false),
                    Orcamento = table.Column<double>(type: "float", nullable: false),
                    StatusProjeto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projetos", x => x.ProjetoId);
                    table.ForeignKey(
                        name: "FK_Projetos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    UtilizadorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuncionarioId = table.Column<int>(type: "int", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.UtilizadorId);
                    table.ForeignKey(
                        name: "FK_Utilizadores_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId");
                    table.ForeignKey(
                        name: "FK_Utilizadores_Funcionarios_FuncionarioId",
                        column: x => x.FuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "FuncionarioId");
                });

            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    ContratoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataInicio = table.Column<DateTime>(type: "date", nullable: false),
                    DataFim = table.Column<DateTime>(type: "date", nullable: false),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    StatusContrato = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ProjetoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.ContratoId);
                    table.ForeignKey(
                        name: "FK_Contratos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contratos_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "ProjetoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FuncionarioProjeto",
                columns: table => new
                {
                    FuncionariosFuncionarioId = table.Column<int>(type: "int", nullable: false),
                    ProjetosProjetoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncionarioProjeto", x => new { x.FuncionariosFuncionarioId, x.ProjetosProjetoId });
                    table.ForeignKey(
                        name: "FK_FuncionarioProjeto_Funcionarios_FuncionariosFuncionarioId",
                        column: x => x.FuncionariosFuncionarioId,
                        principalTable: "Funcionarios",
                        principalColumn: "FuncionarioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuncionarioProjeto_Projetos_ProjetosProjetoId",
                        column: x => x.ProjetosProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "ProjetoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Relatorios",
                columns: table => new
                {
                    RelatorioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataRelatorio = table.Column<DateTime>(type: "date", nullable: false),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    TempoTotalHoras = table.Column<int>(type: "int", nullable: false),
                    ProjetoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorios", x => x.RelatorioId);
                    table.ForeignKey(
                        name: "FK_Relatorios_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "ProjetoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Faturas",
                columns: table => new
                {
                    FaturaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataFatura = table.Column<DateTime>(type: "date", nullable: false),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    ContratoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturas", x => x.FaturaId);
                    table.ForeignKey(
                        name: "FK_Faturas_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "ContratoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ClienteId",
                table: "Contratos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ProjetoId",
                table: "Contratos",
                column: "ProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturas_ContratoId",
                table: "Faturas",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_FuncionarioProjeto_ProjetosProjetoId",
                table: "FuncionarioProjeto",
                column: "ProjetosProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_ClienteId",
                table: "Projetos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_ProjetoId",
                table: "Relatorios",
                column: "ProjetoId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_ClienteId",
                table: "Utilizadores",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_FuncionarioId",
                table: "Utilizadores",
                column: "FuncionarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Faturas");

            migrationBuilder.DropTable(
                name: "FuncionarioProjeto");

            migrationBuilder.DropTable(
                name: "Relatorios");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "Funcionarios");

            migrationBuilder.DropTable(
                name: "Projetos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
