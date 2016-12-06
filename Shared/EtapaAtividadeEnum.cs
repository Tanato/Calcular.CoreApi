using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Shared
{
    public enum EtapaAtividadeEnum
    {
        Original,

        [Description("Revisão")]
        Revisao,

        Refazer,
    }
}
