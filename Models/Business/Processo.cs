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

        public int? FaseProcessoId { get; set; }
        public FaseProcesso FaseProcesso { get; set; }

        public decimal? ValorCausa { get; set; }

        public int AdvogadoId { get; set; }
        public Cliente Advogado { get; set; }

        public string Perito { get; set; }
        public string Indicacao { get; set; }

        public StatusHonorarioEnum? StatusHonorario { get; set; }
        public DateTime? PrazoHonorario { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ProcessoDetalhe> ProcessoDetalhes { get; set; }
        public List<Honorario> Honorarios { get; set; }
        public List<Servico> Servicos { get; set; }
        public List<Cobranca> Cobrancas { get; set; }
        public List<Proposta> Propostas { get; set; }

        /// <summary>
        /// Procedure que Calcula valor pendente do processo.
        /// </summary>
        public decimal? Total { get; set; }

        public decimal? Honorario
        {
            get
            {
                if (Honorarios != null && Honorarios.Count > 0)
                    return Honorarios.Where(x => x.Registro == RegistroEnum.Honorario && !x.Cancelado).Sum(x => x.Valor);
                else
                    return null;
            }
        }

        public DateTime? DataCobranca
        {
            get
            {
                if (Cobrancas != null && Cobrancas.Count > 0)
                    return Cobrancas.OrderByDescending(x => x.Id).First().DataCobranca;
                else
                    return null;
            }
        }

        public DateTime? PrevisaoPagamento
        {
            get
            {
                if (Cobrancas != null && Cobrancas.Count > 0)
                    return Cobrancas.OrderByDescending(x => x.Id).First().PrevisaoPagamento;
                else
                    return null;
            }
        }
<<<<<<< HEAD
=======

        public string StatusHonorario
        {
            get
            {
                if (Honorarios == null || Honorarios.All(x => x.Cancelado) || Honorarios.Count() == 0)
                {
                    return "Vazio";
                }
                else if (Total.HasValue)
                {
                    if (Total <= 0)
                        return "Pago";
                    else if (Prazo.HasValue && Prazo.Value.Date < DateTime.Now.Date)
                        return "Atrasado";
                    else
                        return "Pendente";
                }
                else if (Honorario.HasValue && Honorario.Value == 0
                        && Honorarios != null && !Honorarios.All(x => x.Cancelado))
                {
                    return "Pendente";
                }
                else
                    return string.Empty;
            }
        }
>>>>>>> master
    }
}
