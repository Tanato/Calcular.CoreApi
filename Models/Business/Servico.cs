using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;

namespace Calcular.CoreApi.Models.Business
{
    public class Servico
    {
        public int Id { get; set; }

        public int TipoServicoId { get; set; }
        public TipoServico TipoServico { get; set; }

        public StatusEnum Status { get; set; }
        public string Volumes { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime? Saida { get; set; }
        public DateTime? Prazo { get; set; }

        public TipoImpressaoEnum? TipoImpressao { get; set; }

        public int ProcessoId { get; set; }
        public Processo Processo { get; set; }

        public List<Atividade> Atividades { get; set; }
    }
}
