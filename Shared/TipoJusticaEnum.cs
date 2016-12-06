using Calcular.CoreApi.Common;
using System.ComponentModel;

namespace Calcular.CoreApi.Shared
{
    public enum TipoJusticaEnum
    {
        [Description("Fórum")]
        [Mask("")]
        Forum,
        [Description("Justiça do Trabalho")]
        [Mask("")]
        Trabalho,
        [Description("Justiça Federal")]
        [Mask("")]
        Federal,
        [Description("Outro")]
        Outro,
    }
}