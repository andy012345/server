using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Server.RealmServer
{
    public class Main
    {
        public static void Run()
        {
            if (System.IO.File.Exists("Config-Realm.xml") == false)
                return;

            TextReader configReader = File.OpenText("Config-Realm.xml");

            if (configReader == null)
                throw new Exception("Unable to open Config-Realm.xml");

            var doc = new XmlDocument();
            var xmlReader = XmlReader.Create(configReader);

            doc.Load(xmlReader);

            var els = doc.GetElementsByTagName("Realm");

            foreach (XmlNode node in els)
            {
                var el = node as XmlElement;

                string port = el.GetAttribute("Port");

                if (port == null)
                    throw new Exception("Config-Realm.xml includes a Realm entry that does not specify Port");

                string realmid = el.GetAttribute("RealmID");

                if (realmid == null)
                    throw new Exception("Config-Realm.xml includes a Realm entry that does not specify RealmID");

                string realmname = el.GetAttribute("Name");

                if (realmname == null)
                    throw new Exception("Config-Realm.xml includes a Realm entry that does not specify Name");
                
                string realmaddress = el.GetAttribute("Address");

                if (realmaddress == null)
                    throw new Exception("Config-Realm.xml includes a Realm entry that does not specify Address");

                RealmSettings settings = new RealmSettings();
                settings.ID = int.Parse(realmid);
                settings.RealmID = settings.ID;
                settings.Name = realmname;
                settings.Address = realmaddress;
                settings.Port = UInt16.Parse(port);

                string attrib;
                attrib = el.GetAttribute("MaxPlayers"); if (attrib != null && attrib.Length > 0) settings.MaxPlayers = int.Parse(attrib);
                attrib = el.GetAttribute("RealID"); if (attrib != null && attrib.Length > 0) settings.RealmID = int.Parse(attrib);
                attrib = el.GetAttribute("RequiredAccountLevel"); if (attrib != null && attrib.Length > 0) settings.RequiredAccountLevel = int.Parse(attrib);
                attrib = el.GetAttribute("Category"); if (attrib != null && attrib.Length > 0) settings.Cat = int.Parse(attrib);

                RealmClient client = new RealmClient(settings);
                client.Run();

                //RealmPacketHandler 
            }
        }
    }
}
