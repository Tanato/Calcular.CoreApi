using System.Collections.Generic;

namespace Calcular.CoreApi.Models.Business
{
    public class TipoServico
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        List<Servico> Servicos { get; set; }
    }
}
