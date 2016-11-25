using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Models.Business
{
    public class Honorario
    {
        public int Id { get; set; }
        public RegistroEnum Registro { get; set; }
        public string RegistroDescription
        {
            get
            {
                return EnumHelpers.GetEnumDescription(Registro);
            }
        }

        public TipoRegistroEnum TipoPagamento { get; set; }
        public string TipoPagamentoDescription
        {
            get
            {
                return EnumHelpers.GetEnumDescription(TipoPagamento);
            }
        }

        public string NotaFiscal { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public DateTime? Prazo { get; set; }
        public string Observacao { get; set; }

        public int ProcessoId { get; set; }
        public Processo Processo { get; set; }
    }
}
