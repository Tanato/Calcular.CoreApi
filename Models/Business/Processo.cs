using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;

namespace Calcular.CoreApi.Models.Business
{
    public class Processo
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Autor { get; set; }
        public string Reu { get; set; }
        public LocalEnum Local { get; set; }
        public ParteEnum Parte { get; set; }
        public int NumeroAutores { get; set; }
        public string Vara { get; set; }

        public int AdvogadoId { get; set; }
        public Cliente Advogado { get; set; }

        //public int? PeritoId { get; set; }
        //public User Perito { get; set; }
        
        public int? IndicacaoId { get; set; }
        public User Indicacao { get; set; }

        public decimal? Honorario { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ProcessoDetalhe> ProcessoDetalhes { get; set; }
        public List<Honorario> Honorarios { get; set; }
        public List<Servico> Servicos { get; set; }
    }
}
