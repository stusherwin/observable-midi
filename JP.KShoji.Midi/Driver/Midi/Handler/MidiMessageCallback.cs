/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System.IO;
using Android.OS;
using JP.KShoji.Driver.Midi.Device;
using JP.KShoji.Driver.Midi.Handler;
using JP.KShoji.Driver.Midi.Listener;
using Sharpen;

namespace JP.KShoji.Driver.Midi.Handler
{
	/// <summary>USB MIDI Message parser</summary>
	/// <author>K.Shoji</author>
	public sealed class MidiMessageCallback : Android.OS.Handler.ICallback
	{
		private readonly OnMidiInputEventListener midiEventListener;

		private readonly MidiInputDevice sender;

		private ByteArrayOutputStream received;

		private ByteArrayOutputStream systemExclusive = null;

		/// <summary>constructor</summary>
		/// <param name="device"></param>
		/// <param name="midiEventListener"></param>
		public MidiMessageCallback(MidiInputDevice device, OnMidiInputEventListener midiEventListener
			)
		{
			this.midiEventListener = midiEventListener;
			sender = device;
		}

		public bool HandleMessage(Message msg)
		{
			lock (this)
			{
				if (midiEventListener == null)
				{
					return false;
				}
				if (received == null)
				{
					received = new ByteArrayOutputStream();
				}
				try
				{
                    received.Write((byte[])msg.Obj);
				}
				catch (IOException)
				{
				}
				// ignore exception
				if (received.Size() < 4)
				{
					// more data needed
					return false;
				}
				// USB MIDI data stream: 4 bytes boundary
				byte[] receivedBytes = received.ToByteArray();
				byte[] read = new byte[receivedBytes.Length / 4 * 4];
				System.Array.Copy(receivedBytes, 0, read, 0, read.Length);
				// Note: received.reset() method don't reset ByteArrayOutputStream's internal buffer.
				received = new ByteArrayOutputStream();
				// keep unread bytes
				if (receivedBytes.Length - read.Length > 0)
				{
					byte[] unread = new byte[receivedBytes.Length - read.Length];
					System.Array.Copy(receivedBytes, read.Length, unread, 0, unread.Length);
					try
					{
						received.Write(unread);
					}
					catch (IOException)
					{
					}
				}
				// ignore exception
				int cable;
				int codeIndexNumber;
				int byte1;
				int byte2;
				int byte3;
				for (int i = 0; i < read.Length; i += 4)
				{
					cable = (read[i + 0] >> 4) & unchecked((int)(0xf));
					codeIndexNumber = read[i + 0] & unchecked((int)(0xf));
					byte1 = read[i + 1] & unchecked((int)(0xff));
					byte2 = read[i + 2] & unchecked((int)(0xff));
					byte3 = read[i + 3] & unchecked((int)(0xff));
					switch (codeIndexNumber)
					{
						case 0:
						{
							midiEventListener.OnMidiMiscellaneousFunctionCodes(sender, cable, byte1, byte2, byte3
								);
							break;
						}

						case 1:
						{
							midiEventListener.OnMidiCableEvents(sender, cable, byte1, byte2, byte3);
							break;
						}

						case 2:
						{
							// system common message with 2 bytes
							byte[] bytes = new byte[] { unchecked((byte)byte1), unchecked((byte)byte2) };
							midiEventListener.OnMidiSystemCommonMessage(sender, cable, bytes);
							break;
						}

						case 3:
						{
							// system common message with 3 bytes
							byte[] bytes = new byte[] { unchecked((byte)byte1), unchecked((byte)byte2), unchecked(
								(byte)byte3) };
							midiEventListener.OnMidiSystemCommonMessage(sender, cable, bytes);
							break;
						}

						case 4:
						{
							// sysex starts, and has next
							lock (this)
							{
								if (systemExclusive == null)
								{
									systemExclusive = new ByteArrayOutputStream();
								}
							}
							lock (systemExclusive)
							{
								systemExclusive.Write(byte1);
								systemExclusive.Write(byte2);
								systemExclusive.Write(byte3);
							}
							break;
						}

						case 5:
						{
							// system common message with 1byte
							// sysex end with 1 byte
							if (systemExclusive == null)
							{
								byte[] bytes = new byte[] { unchecked((byte)byte1) };
								midiEventListener.OnMidiSystemCommonMessage(sender, cable, bytes);
							}
							else
							{
								lock (systemExclusive)
								{
									systemExclusive.Write(byte1);
									midiEventListener.OnMidiSystemExclusive(sender, cable, systemExclusive.ToByteArray
										());
								}
								lock (this)
								{
									systemExclusive = null;
								}
							}
							break;
						}

						case 6:
						{
							// sysex end with 2 bytes
							if (systemExclusive != null)
							{
								lock (systemExclusive)
								{
									systemExclusive.Write(byte1);
									systemExclusive.Write(byte2);
									midiEventListener.OnMidiSystemExclusive(sender, cable, systemExclusive.ToByteArray
										());
								}
								lock (this)
								{
									systemExclusive = null;
								}
							}
							break;
						}

						case 7:
						{
							// sysex end with 3 bytes
							if (systemExclusive != null)
							{
								lock (systemExclusive)
								{
									systemExclusive.Write(byte1);
									systemExclusive.Write(byte2);
									systemExclusive.Write(byte3);
									midiEventListener.OnMidiSystemExclusive(sender, cable, systemExclusive.ToByteArray
										());
								}
								lock (this)
								{
									systemExclusive = null;
								}
							}
							break;
						}

						case 8:
						{
							midiEventListener.OnMidiNoteOff(sender, cable, byte1 & unchecked((int)(0xf)), byte2
								, byte3);
							break;
						}

						case 9:
						{
							if (byte3 == unchecked((int)(0x00)))
							{
								midiEventListener.OnMidiNoteOff(sender, cable, byte1 & unchecked((int)(0xf)), byte2
									, byte3);
							}
							else
							{
								midiEventListener.OnMidiNoteOn(sender, cable, byte1 & unchecked((int)(0xf)), byte2
									, byte3);
							}
							break;
						}

						case 10:
						{
							// poly key press
							midiEventListener.OnMidiPolyphonicAftertouch(sender, cable, byte1 & unchecked((int
								)(0xf)), byte2, byte3);
							break;
						}

						case 11:
						{
							// control change
							midiEventListener.OnMidiControlChange(sender, cable, byte1 & unchecked((int)(0xf)
								), byte2, byte3);
							ProcessRpnMessages(cable, byte1, byte2, byte3);
							break;
						}

						case 12:
						{
							// program change
							midiEventListener.OnMidiProgramChange(sender, cable, byte1 & unchecked((int)(0xf)
								), byte2);
							break;
						}

						case 13:
						{
							// channel pressure
							midiEventListener.OnMidiChannelAftertouch(sender, cable, byte1 & unchecked((int)(
								0xf)), byte2);
							break;
						}

						case 14:
						{
							// pitch bend
							midiEventListener.OnMidiPitchWheel(sender, cable, byte1 & unchecked((int)(0xf)), 
								byte2 | (byte3 << 7));
							break;
						}

						case 15:
						{
							// single byte
							midiEventListener.OnMidiSingleByte(sender, cable, byte1);
							break;
						}

						default:
						{
							// do nothing.
							break;
							break;
						}
					}
				}
				return false;
			}
		}

		/// <summary>current RPN status</summary>
		/// <author>K.Shoji</author>
		private enum RPNStatus
		{
			RPN,
			NRPN,
			NONE
		}

		private MidiMessageCallback.RPNStatus rpnStatus = MidiMessageCallback.RPNStatus.NONE;

		private int rpnFunctionMSB = unchecked((int)(0x7f));

		private int rpnFunctionLSB = unchecked((int)(0x7f));

		private int nrpnFunctionMSB = unchecked((int)(0x7f));

		private int nrpnFunctionLSB = unchecked((int)(0x7f));

		private int rpnValueMSB;

		/// <summary>RPN and NRPN messages</summary>
		/// <param name="cable"></param>
		/// <param name="byte1"></param>
		/// <param name="byte2"></param>
		/// <param name="byte3"></param>
		private void ProcessRpnMessages(int cable, int byte1, int byte2, int byte3)
		{
			switch (byte2)
			{
				case 6:
				{
					rpnValueMSB = byte3 & unchecked((int)(0x7f));
					if (rpnStatus == MidiMessageCallback.RPNStatus.RPN)
					{
						midiEventListener.OnMidiRPNReceived(sender, cable, byte1, ((rpnFunctionMSB & unchecked(
							(int)(0x7f))) << 7) & (rpnFunctionLSB & unchecked((int)(0x7f))), rpnValueMSB, -1
							);
					}
					else
					{
						if (rpnStatus == MidiMessageCallback.RPNStatus.NRPN)
						{
							midiEventListener.OnMidiNRPNReceived(sender, cable, byte1, ((nrpnFunctionMSB & unchecked(
								(int)(0x7f))) << 7) & (nrpnFunctionLSB & unchecked((int)(0x7f))), rpnValueMSB, -
								1);
						}
					}
					break;
				}

				case 38:
				{
					if (rpnStatus == MidiMessageCallback.RPNStatus.RPN)
					{
						midiEventListener.OnMidiRPNReceived(sender, cable, byte1, ((rpnFunctionMSB & unchecked(
							(int)(0x7f))) << 7) & (rpnFunctionLSB & unchecked((int)(0x7f))), rpnValueMSB, byte3
							 & unchecked((int)(0x7f)));
					}
					else
					{
						if (rpnStatus == MidiMessageCallback.RPNStatus.NRPN)
						{
							midiEventListener.OnMidiNRPNReceived(sender, cable, byte1, ((nrpnFunctionMSB & unchecked(
								(int)(0x7f))) << 7) & (nrpnFunctionLSB & unchecked((int)(0x7f))), rpnValueMSB, byte3
								 & unchecked((int)(0x7f)));
						}
					}
					break;
				}

				case 98:
				{
					nrpnFunctionLSB = byte3 & unchecked((int)(0x7f));
					rpnStatus = MidiMessageCallback.RPNStatus.NRPN;
					break;
				}

				case 99:
				{
					nrpnFunctionMSB = byte3 & unchecked((int)(0x7f));
					rpnStatus = MidiMessageCallback.RPNStatus.NRPN;
					break;
				}

				case 100:
				{
					rpnFunctionLSB = byte3 & unchecked((int)(0x7f));
					if (rpnFunctionMSB == unchecked((int)(0x7f)) && rpnFunctionLSB == unchecked((int)
						(0x7f)))
					{
						rpnStatus = MidiMessageCallback.RPNStatus.NONE;
					}
					else
					{
						rpnStatus = MidiMessageCallback.RPNStatus.RPN;
					}
					break;
				}

				case 101:
				{
					rpnFunctionMSB = byte3 & unchecked((int)(0x7f));
					if (rpnFunctionMSB == unchecked((int)(0x7f)) && rpnFunctionLSB == unchecked((int)
						(0x7f)))
					{
						rpnStatus = MidiMessageCallback.RPNStatus.NONE;
					}
					else
					{
						rpnStatus = MidiMessageCallback.RPNStatus.RPN;
					}
					break;
				}

				default:
				{
					break;
					break;
				}
			}
		}

        #region IDisposable implementation


        public void Dispose()
        {
        }


        #endregion


        #region IJavaObject implementation


        public System.IntPtr Handle {
            get;
            set;
        }


        #endregion

	}
}
