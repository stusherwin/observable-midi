using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using SetupManager.Core;

namespace SetupManager
{
    public class XmlSetupWriter
    {
        public void Write( List<Setup> setups, string file )
        {
            using (var writer = XmlWriter.Create(file))
            {
                var xml =
                    new XElement("setups", setups.Select(s =>
                        new XElement("setup",
                            new XAttribute("name", s.Name),
                            new XAttribute("rdname", s.RDName),
                            s.SysExMessages.Select(m =>
                                new XElement("sysex",
                                    String.Join(" ", m.Bytes.Select(b => b.ToString("X2")))
                                )
                            )
                        )
                    ));

                xml.WriteTo(writer);
            }
        }
    }
}
