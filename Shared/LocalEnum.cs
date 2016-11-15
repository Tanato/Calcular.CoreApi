using System.ComponentModel;

namespace Calcular.CoreApi.Shared
{
    public enum LocalEnum
    {
        [Description("Fórum")]
        Forum,
        [Description("Justiça do Trabalho")]
        Trabalho,
        [Description("Justiça Federal")]
        Federal,
    }
}