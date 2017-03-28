using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colso.Xrm.DataTransporter.Models
{

    public class Item<K, V>
    {
        public Item() { }

        public Item(K key, V value)
        {
            this.Key = key;
            this.Value = value;
        }

        public K Key;

        public V Value;
    }
}
