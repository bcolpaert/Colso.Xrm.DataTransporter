using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colso.Xrm.DataTransporter.Models
{
    public class EntitySetting
    {

        public List<Item<EntityReference, EntityReference>> Mappings;

        public List<string> UnmarkedAttributes { get; set; }

        public string Filter { get; set; }
    }
}
