﻿using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Telefone2 { get; set; }
        public string Celular { get; set; }
        public string Celular2 { get; set; }
        public DateTime? Nascimento { get; set; }
        public PerfilEnum? Perfil { get; set; }
        public string Empresa { get; set; }
        public decimal? Honorarios { get; set; }
        public ComoChegouEnum? ComoChegou { get; set; }
        public string ComoChegouDetalhe { get; set; }
        public string Observacao { get; set; }
        public int Processos { get { return 0; } }
    }
}
