using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Shared
{
    public enum TipoRegistroEnum
    {
        // Honorário
        NotaFiscal,
        Recibo,
        Contrato,
        Alvara,

        // Pagamento
        ChequeAVista,
        ChequeAPrazo,
        Dinheiro,
        DepositoBancario,
    }
}
