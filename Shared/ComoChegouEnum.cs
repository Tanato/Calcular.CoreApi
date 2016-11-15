using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Shared
{
    public enum ComoChegouEnum
    {
        [Description("Internet")]
        Internet,
        [Description("Galeria")]
        Galeria,
        [Description("Indicação")]
        Indicacao,
        [Description("Outro")]
        Outro
    }
}
