using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Shared
{
    public enum PerfilEnum
    {
        [Description("Advogado")]
        Advogado,

        [Description("Juiz")]
        Juiz,

        [Description("Diretor Secretaria")]
        DiretorSecretaria,

        [Description("Outro")]
        Outro,
    }
}
