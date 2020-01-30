using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colso.Xrm.DataTransporter.AppCode
{
    public static class Enumerations
    {
        public enum TransferMode
        {
            None = 0,
            Preview = 1,
            Create = 2,
            Update = 4,
            Delete = 8
        }
    }
}
