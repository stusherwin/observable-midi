using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetupManager.Core
{
    public class Setup
    {
        public string Name { get; set; }
        public string RDName { get; private set; }
        public List<SysExMessage> SysExMessages { get; private set; }

        public static Setup Load(IEnumerable<SysExMessage> sysExMessages)
        {
            var messages = sysExMessages.ToList();
            var name = messages.First(m => m.IsNameMessage).Name;

            return new Setup
            {
                Name = name,
                RDName = name,
                SysExMessages = messages
            };
        }
    }
}