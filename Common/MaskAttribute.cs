using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calcular.CoreApi.Common
{
    public class MaskAttribute : Attribute
    {
        public MaskAttribute(string mask)
        {
            DescriptionValue = mask;
        }

        public virtual string Description => DescriptionValue;
        protected string DescriptionValue { get; set; }
    }
}
