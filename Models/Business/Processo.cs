using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calcular.CoreApi.Models.Business
{
    public class Processo
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string Autor { get; set; }
        public string Reu { get; set; }
        public TipoJusticaEnum Local { get; set; }
        public ParteEnum Parte { get; set; }
        public int NumeroAutores { get; set; }
        public string Vara { get; set; }

        public int AdvogadoId { get; set; }
        public Cliente Advogado { get; set; }

        public string PeritoId { get; set; }
        public User Perito { get; set; }
        
        public string IndicacaoId { get; set; }
        public User Indicacao { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ProcessoDetalhe> ProcessoDetalhes { get; set; }
        public List<Honorario> Honorarios { get; set; }
        public List<Servico> Servicos { get; set; }
        public List<Cobranca> Cobrancas { get; set; }
        public List<Proposta> Propostas { get; set; }

        public decimal? Total
        {
            get
            {
                if (Honorarios != null && Honorarios.Count > 0)
                {
                    var positivo = Honorarios.Where(x => x.Registro == RegistroEnum.Honorario).Sum(x => x.Valor);
                    var negativo = Honorarios.Where(x => x.Registro == RegistroEnum.Pagamento).Sum(x => x.Valor);

                    return positivo - negativo;
                }
                else
                    return null;
            }
        }

        public decimal? Honorario
        {
            get
            {
                if (Honorarios != null && Honorarios.Count > 0)
                    return Honorarios.Where(x => x.Registro == RegistroEnum.Honorario).Sum(x => x.Valor);
                else
                    return null;
            }
        }

        public DateTime? Prazo
        {
            get
            {
                if (Honorarios != null && Honorarios.Count > 0)
                    return Honorarios.Where(x => x.Registro == RegistroEnum.Honorario).Max(x => x.Prazo);
                else
                    return null;
            }
        }
    }
}
