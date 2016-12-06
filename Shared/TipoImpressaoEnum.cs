using System.ComponentModel;

namespace Calcular.CoreApi.Shared
{
    public enum TipoImpressaoEnum
    {
        Word,
        Excel,
        [Description("Word + Excel")]
        WordExcel,
    }
}
