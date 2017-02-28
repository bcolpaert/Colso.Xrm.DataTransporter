using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Args;
using XrmToolBox.Extensibility.Interfaces;

namespace Colso.DataTransporter.AppCode
{
    public class SettingFileHandler
    {
        public static bool GetConfigData(out Settings config)
        {
            var allok = SettingsManager.Instance.TryLoad<Settings>(typeof(DataTransporter), out config);

            if (config == null)  config = new Settings();

            return allok;
        }

        public static bool SaveConfigData(Settings config)
        {
            try
            {
                SettingsManager.Instance.Save(typeof(DataTransporter), config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

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

    public class Settings
    {
        public Settings() { }

        private List<Item<Guid, Organisations>> _Organisations;
        public List<Item<Guid, Organisations>> Organisations
        {
            get { return _Organisations; }
            set { _Organisations = value; }
        }

        public Organisations this[Guid organisationid] 
        {
            get
            {
                if (_Organisations == null)
                    _Organisations = new List<Item<Guid, Organisations>>();

                if (!_Organisations.Any(o => o.Key == organisationid))
                    _Organisations.Add(new Item<Guid, Organisations>(organisationid, new Organisations()));

                return _Organisations.Where(o => o.Key == organisationid).Select(o => o.Value).FirstOrDefault();
            }
        }
    }

    public class Organisations
    {
        public Organisations()
        {
            this.Sortcolumns = new List<Item<string, int>>();
            this.Mappings = new List<Item<EntityReference, EntityReference>>();
            this.Entities = new List<Item<string, EntitySettings>>();
        }

        private List<Item<string, int>> _Sortcolumns;
        public List<Item<string, int>> Sortcolumns
        {
            get { return _Sortcolumns; }
            set { _Sortcolumns = value; }
        }

        private List<Item<EntityReference, EntityReference>> _Mappings;
        public List<Item<EntityReference, EntityReference>> Mappings
        {
            get { return _Mappings; }
            set { _Mappings = value; }
        }

        private List<Item<string, EntitySettings>> _Entities;
        public List<Item<string, EntitySettings>> Entities
        {
            get { return _Entities; }
            set { _Entities = value; }
        }

        public EntitySettings this[string logicalname]
        {
            get
            {
                if (_Entities == null)
                    _Entities = new List<Item<string, EntitySettings>>();

                if (!_Entities.Any(o => o.Key == logicalname))
                    _Entities.Add(new Item<string, EntitySettings>(logicalname, new EntitySettings()));

                return _Entities.Where(o => o.Key == logicalname).Select(o => o.Value).FirstOrDefault();
            }
        }
    }

    public class EntitySettings
    {
        public EntitySettings()
        {
            this.UnmarkedAttributes = new List<string>();
            this.Filter = string.Empty;
        }

        public List<string> UnmarkedAttributes { get; set; }

        public string Filter { get; set; }
    }
}
