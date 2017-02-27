using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class ComissaoAtividade
    {
        public int Id { get; set; }
            
        public int AtividadeId { get; set; }
        public Atividade Atividade { get; set; }

        public int ComissaoFuncionarioMesId { get; set; }
        public ComissaoFuncionarioMes ComissaoFuncionarioMes { get; set; }

        public decimal? ValorBase { get; set; }
        public decimal? ValorAdicional { get; set; }
        public decimal? ValorFinal { get; set; }
    }
}
