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

namespace Colso.DataTransporter.AppCode
{
    public class SettingsManager
    {
        // name of the .xml file
        public static string CONFIG_FNAME = "Colso.DataTransporter.Settings.xml";

        public static bool GetConfigData(out Settings config)
        {
            var allok = true;

            if (!File.Exists(CONFIG_FNAME)) // create config file with default values
            {
                config = CreateConfigDataFile();
            }
            else // try to read configuration from file
            {
                try
                {
                    var serializer = new DataContractSerializer(typeof(Settings));
                    using (FileStream fs = new FileStream(CONFIG_FNAME, FileMode.Open))
                    {
                        config = (Settings)serializer.ReadObject(fs);
                    }
                }
                catch (Exception ex)
                {
                    allok = false;
                    // Create
                    config = CreateConfigDataFile();
                }
            }

            return allok;
        }

        public static bool SaveConfigData(Settings config)
        {
            if (!File.Exists(CONFIG_FNAME)) return false; // don't do anything if file doesn't exist

            try
            {
                var serializer = new DataContractSerializer(typeof(Settings));
                using (FileStream fs = new FileStream(CONFIG_FNAME, FileMode.Open))
                {
                    using (var writer = new XmlTextWriter(fs, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                        serializer.WriteObject(writer, config);
                        writer.Flush();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Writing to file failed!
                return false;
            }
        }

        private static Settings CreateConfigDataFile()
        {
            Settings sxml = new Settings();

            try
            {
                using (FileStream fs = new FileStream(CONFIG_FNAME, FileMode.Create))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Settings));
                    xs.Serialize(fs, sxml);
                }
            }
            catch (Exception)
            {
                // Writing to file failed!
            }

            return sxml;
        }
    }

    [DataContract]
    public class Settings
    {
        [DataMember]
        private Dictionary<Guid, OrganisationSettings> Organisations { get; set; }

        public OrganisationSettings this[Guid organisationid] 
        {
            get
            {
                if (this.Organisations == null)
                    this.Organisations = new Dictionary<Guid, OrganisationSettings>();

                if (!this.Organisations.ContainsKey(organisationid))
                    this.Organisations.Add(organisationid, new OrganisationSettings());

                return this.Organisations[organisationid];
            }
        }
    }

    [DataContract]
    public class OrganisationSettings
    {
        public OrganisationSettings()
        {
            this.Sortcolumns = new Dictionary<string, int>();
            this.Mappings = new List<Tuple<EntityReference, EntityReference>>();
            this.Entities = new Dictionary<string, EntitySettings>();
        }

        [DataMember]
        public Dictionary<string, int> Sortcolumns { get; set; }

        [DataMember]
        public List<Tuple<EntityReference, EntityReference>> Mappings { get; set; }

        [DataMember]
        private Dictionary<string, EntitySettings> Entities { get; set; }

        public EntitySettings this[string logicalname]
        {
            get
            {
                if (this.Entities == null)
                    this.Entities = new Dictionary<string, EntitySettings>();

                if (!this.Entities.ContainsKey(logicalname))
                    this.Entities.Add(logicalname, new EntitySettings());

                return this.Entities[logicalname];
            }
        }
    }

    [DataContract]
    public class EntitySettings
    {
        public EntitySettings()
        {
            this.UnmarkedAttributes = new List<string>();
            this.Filter = string.Empty;
        }

        [DataMember]
        public List<string> UnmarkedAttributes { get; set; }

        [DataMember]
        public string Filter { get; set; }
    }
}
