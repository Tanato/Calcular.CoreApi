using Calcular.CoreApi.Models.Business;
using Calcular.CoreApi.Shared;
using System;

namespace Calcular.CoreApi.Models.ViewModels
{
    public class AtividadeViewModel
    {
        public int Id { get; set; }
        public DateTime? Entrega { get; set; }
        public string Tempo { get; set; }
        public TipoImpressaoEnum? TipoImpressao { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoRevisor { get; set; }
        public string ObservacaoComissao { get; set; }

        public int TipoAtividadeId { get; set; }
        public TipoAtividade TipoAtividade { get; set; }

        public int ServicoId { get; set; }
        public Servico Servico { get; set; }

        public string ResponsavelId { get; set; }
        public User Responsavel { get; set; }

        public TipoExecucaoEnum TipoExecucao { get; set; }

        public int? AtividadeOrigemId { get; set; }
        public Atividade AtividadeOrigem { get; set; }

        public EtapaAtividadeEnum EtapaAtividade { get; set; }
    }
}
