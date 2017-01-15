using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class Comissao
    {
        public int Id { get; set; }
        public int TipoAtividadeId { get; set; }
        public TipoAtividade TipoAtividade { get; set; }

        public decimal HoraMinima { get; set; }
        public decimal HoraMaxima { get; set; }
        public decimal Valor { get; set; }

        public bool Ativo { get; set; }

        public DateTime Vigencia { get; set; }
    }
}
