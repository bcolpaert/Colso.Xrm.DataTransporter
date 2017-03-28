using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colso.Xrm.DataTransporter.Models
{

    public class Mapping
    {
        public Mapping() { }

        public Mapping(string logicalName, Guid from, Guid to)
        {
            this.LogicalName = logicalName;
            this.From = from;
            this.To = to;
        }

        public bool Global;

        public string LogicalName;

        public Guid From;

        public Guid To;
    }
}
