using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class ProcessoDetalhe
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int ProcessoId { get; set; }
        public Processo Processo { get; set; }

        public DateTime Data { get; set; }
    }
}