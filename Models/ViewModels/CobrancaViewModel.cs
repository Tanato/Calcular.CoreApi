using Calcular.CoreApi.Models.Business;
using System;
using System.Collections.Generic;

namespace Calcular.CoreApi.Models.ViewModels
{
    public class CobrancaViewModel
    {
        public int AdvogadoId { get; set; }
        public Cliente Advogado { get; set; }

        public DateTime? DataCobranca { get; set; }

        public int TotalProcessosPendentes { get; set; }
        public decimal TotalHonorarios { get; set; }
        public decimal TotalPendente { get; set; }
        public string StatusHonorario { get; set; }

        public IEnumerable<ProcessoViewModel> Processos { get; set; }
    }
}
