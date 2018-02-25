using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colso.Xrm.DataTransporter.Models
{
    class RecordStateAndStatus
    {
        public OptionSetValue State { get; set; }
        public OptionSetValue Status { get; set; }
    }
}
