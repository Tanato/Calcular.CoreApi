using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;

namespace Calcular.CoreApi.Models.ViewModels
{
    public class ProcessoViewModel
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Autor { get; set; }
        public string Reu { get; set; }
        public TipoJusticaEnum Local { get; set; }
        public ParteEnum Parte { get; set; }
        public int NumeroAutores { get; set; }
        public string Vara { get; set; }

        public int? FaseProcessoId { get; set; }
        public FaseProcesso FaseProcesso { get; set; }

        public int AdvogadoId { get; set; }
        public Cliente Advogado { get; set; }
        
        public string Perito { get; set; }
        public string Indicacao { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ProcessoDetalhe> ProcessoDetalhes { get; set; }
        public List<Honorario> Honorarios { get; set; }
        public List<Servico> Servicos { get; set; }
        public List<Proposta> Propostas { get; set; }

        public List<Cobranca> Cobrancas { get; set; }
        public Cobranca UltimaCobranca { get; set; }

        public decimal? Total { get; set; }
        public decimal? Honorario { get; set; }
        public decimal? CustoComissao { get; set; }
        public DateTime? Prazo { get; set; }

        public string StatusHonorario { get; set; }
        public DateTime? PrevisaoPagamento { get; set; }
        public DateTime? DataCobranca { get; set; }
    }
}
