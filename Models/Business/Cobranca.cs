using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class Cobranca
    {
        public int Id { get; set; }
        public int ProcessoId { get; set; }
        public Processo Processo { get; set; }

        public decimal ValorPendente { get; set; }
        public string Contato { get; set; }
        public DateTime DataCobranca { get; set; }
        public DateTime? PrevisaoPagamento { get; set; }
        public string Observacao { get; set; }

        public string UsuarioId { get; set; }
        public User Usuario { get; set; }
    }
}
