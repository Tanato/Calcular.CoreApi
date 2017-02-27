using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calcular.CoreApi.Models.Business
{
    public class Comissao
    {
        public int Id { get; set; }
        public int TipoAtividadeId { get; set; }
        public TipoAtividade TipoAtividade { get; set; }
        
        public decimal Valor { get; set; }
        public bool Ativo { get; set; }
        public DateTime Vigencia { get; set; }
        
        [Obsolete("Property HoraMin should be used instead.")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long? HoraMinTicks { get; set; }

        [NotMapped]
        public TimeSpan? HoraMin
        {
#pragma warning disable 618
            get { return HoraMinTicks.HasValue ? new TimeSpan(HoraMinTicks.Value) : (TimeSpan?)null; }
            set { HoraMinTicks = value.HasValue ? value.Value.Ticks : (long?)null; }
#pragma warning restore 618
        }

        [Obsolete("Property HoraMax should be used instead.")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long? HoraMaxTicks { get; set; }

        [NotMapped]
        public TimeSpan? HoraMax
        {
#pragma warning disable 618
            get { return HoraMaxTicks.HasValue ? new TimeSpan(HoraMaxTicks.Value) : (TimeSpan?)null; }
            set { HoraMaxTicks = value.HasValue ? value.Value.Ticks : (long?)null; }
#pragma warning restore 618
        }
    }
}
