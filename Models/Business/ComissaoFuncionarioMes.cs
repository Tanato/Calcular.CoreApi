using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;
namespace Calcular.CoreApi.Models.Business
{
    public class ComissaoFuncionarioMes
    {
        public int Id { get; set; }

        public string ResponsavelId { get; set; }
        public User Responsavel { get; set; }

        public string FuncionarioId { get; set; }
        public User Funcionario { get; set; }

        public DateTime Alteracao { get; set; }

        public int Mes { get; set; }
        public int Ano { get; set; }

        public StatusApuracaoComissao Status { get; set; }

        public ICollection<ComissaoAtividade> ComissaoAtividades { get; set; }
    }
}
