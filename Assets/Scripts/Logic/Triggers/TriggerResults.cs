using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine.Triggers;

namespace MVZ2Logic.Triggers
{
    public class TriggerResultBooleanMessage : TriggerResult<bool>
    {
        public string Message { get; set; }
    }
}
