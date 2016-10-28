﻿using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;

namespace Calcular.CoreApi.Models.Business
{
    public class Processo
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Reclamante { get; set; }
        public string Reclamada { get; set; }
        public LocalEnum Local { get; set; }
        public ParteEnum Parte { get; set; }
        public string Advogado { get; set; }

        public int PeritoId { get; set; }
        public User Perito { get; set; }

        public decimal Honorario { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ProcessoDetalhe> ProcessoDetalhes { get; set; }
        public List<Honorario> Honorarios { get; set; }
        public List<Servico> Servicos { get; set; }
    }
}