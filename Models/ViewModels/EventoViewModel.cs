using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.ViewModels
{
    public class EventoViewModel
    {
        public int Id { get; set; }
        public string Evento { get; set; }
        public DateTime Data { get; set; }
        public string Detalhe { get; set; }
        public string Observacao { get; set; }
        public string Responsavel { get; set; }
        public string Status { get; set; }
    }
}
