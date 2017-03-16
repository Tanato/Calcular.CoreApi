using Calcular.CoreApi.Shared;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calcular.CoreApi.Models.Business
{
    public class Atividade
    {
        public int Id { get; set; }
        public DateTime? Entrega { get; set; }
        
        [Obsolete("Property Tempo should be used instead.")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long? TempoTicks { get; set; }

        [NotMapped]
        public TimeSpan? Tempo
        {
#pragma warning disable 618
            get { return TempoTicks.HasValue ? new TimeSpan(TempoTicks.Value) : (TimeSpan?)null; }
            set { TempoTicks = value.HasValue ? value.Value.Ticks : (long?)null; }
#pragma warning restore 618
        }
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
        
        public ComissaoAtividade ComissaoAtividade { get; set; }

        public EtapaAtividadeEnum EtapaAtividade { get; set; }

        public decimal? Valor { get; set; }
    }
}