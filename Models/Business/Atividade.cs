using Calcular.CoreApi.Shared;
using System;

namespace Calcular.CoreApi.Models.Business
{
    public class Atividade
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime? Entrega { get; set; }
        public decimal? Tempo { get; set; }
        public TipoImpressaoEnum? TipoImpressao { get; set; }
        public string Observacao { get; set; }

        public int TipoAtividadeId { get; set; }
        public TipoAtividade TipoAtividade { get; set; }

        public int ServicoId { get; set; }
        public Servico Servico { get; set; }

        public string ResponsavelId { get; set; }
        public User Responsavel { get; set; }

        public string Status
        {
            get
            {
                if (Tempo != null || !string.IsNullOrEmpty(Observacao) || Entrega != null)
                    return "Em Execução";
                else
                    return "Pendente";
            }
        }
    }
}