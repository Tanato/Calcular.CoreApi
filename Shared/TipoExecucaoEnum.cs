using System.ComponentModel;

namespace Calcular.CoreApi.Shared
{
    public enum TipoExecucaoEnum
    {
        Pendente,

        Finalizado,

        Revisar,

        Refazer,

        Cancelado,

        [Description("Revisar Cancelado")]
        RevisarCancelado,

        [Description("Refazer Cancelado")]
        RefazerCancelado,
    }
}
