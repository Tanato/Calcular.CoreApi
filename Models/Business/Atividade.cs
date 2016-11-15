using Calcular.CoreApi.Shared;
using System;

namespace Calcular.CoreApi.Models.Business
{
    public class Atividade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Entrega { get; set; }
        public decimal Tempo { get; set; }
        public TipoImpressaoEnum TipoImpressao { get; set; }
        public string Observacao { get; set; }

        public int TipoAtividadeId { get; set; }
        public TipoAtividade TipoAtividade { get; set; }

        public int ServicoId { get; set; }
        public Servico Servico { get; set; }

        public int ResponsavelId { get; set; }
        public User Responsavel { get; set; }

    }
}