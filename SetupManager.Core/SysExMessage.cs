using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetupManager.Core
{
    public class SysExMessage
    {
        private static readonly byte[] NAME_MESSAGE_ADDRESS = new byte[] { 0x10, 0x00, 0x00, 0x00 };
        public byte[] Bytes { get; private set; }

        public SysExMessage(byte[] bytes)
        {
            Bytes = bytes;
        }

        public IEnumerable<byte> Address
        {
            get { return Bytes.Skip(7).Take(4); }
        }

        public bool IsNameMessage
        {
            get
            {
                return Address.SequenceEqual(NAME_MESSAGE_ADDRESS);
            }
        }

        public string Name
        {
            get
            {
                return IsNameMessage ?
                    Encoding.ASCII.GetString(Bytes.Skip(11).Take(12).ToArray())
                  : string.Empty;
            }
        }
        
    }
}