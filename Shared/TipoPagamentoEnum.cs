using System.ComponentModel;

namespace Calcular.CoreApi.Shared
{
    public enum TipoRegistroEnum
    {
        // Honorário
        [Description("Nota Fiscal")]
        NotaFiscal,

        [Description("Recibo")]
        Recibo,

        [Description("Contrato")]
        Contrato,

        [Description("Alvará")]
        Alvara,

        // Pagamento
        [Description("Cheque à vista")]
        ChequeAVista,

        [Description("Cheque à prazo")]
        ChequeAPrazo,

        [Description("Dinheiro")]
        Dinheiro,

        [Description("Depósito bancário")]
        DepositoBancario,
    }
}
