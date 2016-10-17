using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class TipoAtividade
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        List<Atividade> Atividades { get; set; }
    }
}
