using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetupManager.Core
{
    public class SysExMessage
    {
        private static readonly byte[] NAME_MESSAGE_ADDRESS = new byte[] { 0x10, 0x00, 0x00, 0x00 };
		private const int ADDRESS_START = 8;
		private const int ADDRESS_LENGTH = 12;
		private const int NAME_START = 12;
		private const int NAME_LENGTH = 12;

        public byte[] Bytes { get; private set; }

        public SysExMessage(byte[] bytes)
        {
            Bytes = bytes;
        }

        public byte[] Address
        {
			get { return GetBytes(ADDRESS_START, ADDRESS_LENGTH); }
        }

        public bool IsNameMessage
        {
            get { return Address.SequenceEqual(NAME_MESSAGE_ADDRESS); }
        }

        public string Name
        {
            get
            {
                return IsNameMessage ?
                    Encoding.ASCII.GetString(GetBytes(NAME_START, NAME_LENGTH))
                  : string.Empty;
            }
        }

		private byte[] GetBytes(int start, int length) {
		    return Bytes.Skip(start - 1).Take(length).ToArray();
		}
    }
}