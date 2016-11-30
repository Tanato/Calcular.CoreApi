using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class Proposta
    {
        public int Id { get; set; }
        public int ProcessoId { get; set; }
        public Processo Processo { get; set; }

        public int Honorario { get; set; }
        public DateTime DataProposta { get; set; }
        public ComoChegouEnum ComoChegou { get; set; }
        public string Observacao { get; set; }
    }
}
