using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class SalarioMinimo
    {
        public int Id { get; set; }
        public int Ano { get; set; }
        public DateTime Vigencia { get; set; }
        public decimal Valor { get; set; }
    }
}
