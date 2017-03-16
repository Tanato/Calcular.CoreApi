using System.ComponentModel;

namespace Calcular.CoreApi.Shared
{
    public enum StatusHonorarioEnum
    {
        [Description("   -   ")]
        Undefined,
        Pendente,
        Pago,
        Atrasado
    }
}
