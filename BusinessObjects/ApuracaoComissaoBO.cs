using Calcular.CoreApi.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.BusinessObjects
{
    public static class ApuracaoComissaoBO
    {
        public static decimal GetValorBase(IEnumerable<Comissao> comissoes, Atividade atividade)
        {
            // Se atividade possui valor, considera como valor base.
            if (atividade.Valor.HasValue)
                return atividade.Valor.Value;
            else
            {
                var comissao = comissoes.Where(x => x.Ativo
                                                    && x.Vigencia <= atividade.Entrega
                                                    && x.TipoAtividadeId == atividade.TipoAtividadeId
                                                    && atividade.Tempo <= (x.HoraMax != null ? x.HoraMax.Value : TimeSpan.MaxValue)
                                                    && atividade.Tempo >= (x.HoraMin != null ? x.HoraMin.Value : TimeSpan.MinValue))
                         .OrderByDescending(x => x.Vigencia)
                         .FirstOrDefault();

                return comissao != null ? comissao.Valor : 0;
            }
        }
    }
}
