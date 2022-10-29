using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParameterTools.CopyParameters
{
    /// <summary>
    /// A class used for creation bindings
    /// </summary>
    public class BoolWrapper
    {
        internal BoolWrapper()
        {
            Value = false;
        }
        public bool Value { get; set; }
    }

}
