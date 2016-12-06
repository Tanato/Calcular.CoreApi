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
        [Description("Demora Retorno")]
        DemoraRetorno,
        [Description("Outro")]
        Outro,
    }
}
