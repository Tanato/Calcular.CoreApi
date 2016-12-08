using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Shared
{
    public enum MotivoEnum
    {
        [Description("Valor")]
        Valor,
        [Description("Tempo de Retorno")]
        DemoraRetorno,
        [Description("Outro")]
        Outro,
    }
}
