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
        
        public string Perito { get; set; }
        public string Indicacao { get; set; }

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
                    //ToDo
                    //var listPositivo = Honorarios.Where(x => x.Registro == RegistroEnum.Honorario && !x.Cancelado && x.Valor.HasValue);
                    //var positivo = listPositivo.Count() > 0 ? listPositivo.Sum(x => x.Valor) : null;

                    //var listNegativo = Honorarios.Where(x => x.Registro == RegistroEnum.Pagamento && !x.Cancelado && x.Valor.HasValue);
                    //var negativo = listNegativo.Count() > 0 ? listNegativo.Sum(x => x.Valor) : null;

                    var positivo = Honorarios.Where(x => x.Registro == RegistroEnum.Honorario && !x.Cancelado).Sum(x => x.Valor);
                    var negativo = Honorarios.Where(x => x.Registro == RegistroEnum.Pagamento && !x.Cancelado).Sum(x => x.Valor);

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
                    return Honorarios.Where(x => x.Registro == RegistroEnum.Honorario && !x.Cancelado).Sum(x => x.Valor);
                else
                    return null;
            }
        }

        public DateTime? Prazo
        {
            get
            {
                if (Honorarios != null && Honorarios.Count > 0)
                    return Honorarios.Where(x => x.Registro == RegistroEnum.Honorario && !x.Cancelado).Max(x => x.Prazo);
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

        public string StatusHonorario
        {
            get
            {
                if (Honorarios != null && Honorarios.Count > 0)
                {
                    if (Total <= 0)
                        return "Pago";
                    else if (Prazo.HasValue && Prazo.Value.Date < DateTime.Now.Date)
                        return "Atrasado";
                    else
                        return "Pendente";
                }
                else
                    return null;
            }
        }
    }
}
