using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Server.RealmServer
{
    class Main
    {
        public void Run()
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

                //RealmPacketHandler 
            }
        }
    }
}
