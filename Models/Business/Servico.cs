using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class Servico
    {
        public int Id { get; set; }
        
        public StatusEnum Status { get; set; }
        public int Volumes { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Saida { get; set; }
        public DateTime Prazo { get; set; }

        public int ProcessoId { get; set; }
        public Processo Processo { get; set; }

        public List<Atividade> Atividades { get; set; }


    }
}
