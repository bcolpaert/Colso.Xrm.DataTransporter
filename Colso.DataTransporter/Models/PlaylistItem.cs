using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static Colso.Xrm.DataTransporter.AppCode.Enumerations;

namespace Colso.Xrm.DataTransporter.Models
{
    public class PlaylistItem
    {
        public int Sequence;

        [XmlIgnore]
        public TransferMode Actions;

        public EntitySetting Setting;

        public int ActionValue
        {
            get
            {
                return (int)Actions;
            }

            set
            {
                Actions = (TransferMode)value;
            }
        }
    }
}
