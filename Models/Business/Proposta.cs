using Calcular.CoreApi.Shared;
using System;
using System.Collections.Generic;

namespace Calcular.CoreApi.Models.Business
{
    public class Proposta
    {
        public int Id { get; set; }

        public string Numero { get; set; }
        public string Contato { get; set; }
        public int ContatoId { get; set; }
        public TipoJusticaEnum Local { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string Telefone { get; set; }
        
        public ComoChegouEnum ComoChegou { get; set; }
        public string ComoChegouDetalhe { get; set; }

        public int Honorario { get; set; }
        public DateTime? DataProposta { get; set; }
        public string Observacao { get; set; }

        public bool Fechado { get; set; }
        public MotivoEnum Motivo { get; set; }
        public string MovitoDetalhe { get; set; }

        public string UsuarioId { get; set; }
        public User Usuario { get; set; }

        public int? ProcessoId { get; set; }
        public Processo Processo { get; set; }
    }
}
