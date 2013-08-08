/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System.Collections.Generic;
using JP.KShoji.Javax.Sound.Midi;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi
{
	public abstract class MidiDevice
	{
		public abstract MidiDevice.Info GetDeviceInfo();

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public abstract void Open();

		public abstract void Close();

		public abstract bool IsOpen();

		public abstract long GetMicrosecondPosition();

		public abstract int GetMaxReceivers();

		public abstract int GetMaxTransmitters();

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public abstract Receiver GetReceiver();

		public abstract IList<Receiver> GetReceivers();

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public abstract Transmitter GetTransmitter();

		public abstract IList<Transmitter> GetTransmitters();

		public class Info
		{
			private string name;

			private string vendor;

			private string description;

			private string version;

			public Info(string name, string vendor, string description, string version)
			{
				this.name = name;
				this.vendor = vendor;
				this.description = description;
				this.version = version;
			}

			public string GetName()
			{
				return name;
			}

			public string GetVendor()
			{
				return vendor;
			}

			public string GetDescription()
			{
				return description;
			}

			public string GetVersion()
			{
				return version;
			}

			public sealed override string ToString()
			{
				return name;
			}

			public override int GetHashCode()
			{
				int prime = 31;
				int result = 1;
				result = prime * result + ((description == null) ? 0 : description.GetHashCode());
				result = prime * result + ((name == null) ? 0 : name.GetHashCode());
				result = prime * result + ((vendor == null) ? 0 : vendor.GetHashCode());
				result = prime * result + ((version == null) ? 0 : version.GetHashCode());
				return result;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				if (obj == null)
				{
					return false;
				}
				if (GetType() != obj.GetType())
				{
					return false;
				}
				MidiDevice.Info other = (MidiDevice.Info)obj;
				if (description == null)
				{
					if (other.description != null)
					{
						return false;
					}
				}
				else
				{
					if (!description.Equals(other.description))
					{
						return false;
					}
				}
				if (name == null)
				{
					if (other.name != null)
					{
						return false;
					}
				}
				else
				{
					if (!name.Equals(other.name))
					{
						return false;
					}
				}
				if (vendor == null)
				{
					if (other.vendor != null)
					{
						return false;
					}
				}
				else
				{
					if (!vendor.Equals(other.vendor))
					{
						return false;
					}
				}
				if (version == null)
				{
					if (other.version != null)
					{
						return false;
					}
				}
				else
				{
					if (!version.Equals(other.version))
					{
						return false;
					}
				}
				return true;
			}
		}
	}
}
