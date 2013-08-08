/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using JP.KShoji.Javax.Sound.Midi;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi
{
	public class ShortMessage : MidiMessage
	{
		public const int NOTE_OFF = unchecked((int)(0x80));

		public const int NOTE_ON = unchecked((int)(0x90));

		public const int POLY_PRESSURE = unchecked((int)(0xa0));

		public const int CONTROL_CHANGE = unchecked((int)(0xb0));

		public const int PROGRAM_CHANGE = unchecked((int)(0xc0));

		public const int CHANNEL_PRESSURE = unchecked((int)(0xd0));

		public const int PITCH_BEND = unchecked((int)(0xe0));

		public const int MIDI_TIME_CODE = unchecked((int)(0xf1));

		public const int SONG_POSITION_POINTER = unchecked((int)(0xf2));

		public const int SONG_SELECT = unchecked((int)(0xf3));

		public const int TUNE_REQUEST = unchecked((int)(0xf6));

		public const int END_OF_EXCLUSIVE = unchecked((int)(0xf7));

		public const int TIMING_CLOCK = unchecked((int)(0xf8));

		public const int START = unchecked((int)(0xfa));

		public const int CONTINUE = unchecked((int)(0xfb));

		public const int STOP = unchecked((int)(0xfc));

		public const int ACTIVE_SENSING = unchecked((int)(0xfe));

		public const int SYSTEM_RESET = unchecked((int)(0xff));

		public ShortMessage() : this(new byte[] { unchecked((byte)NOTE_ON), unchecked((int
			)(0x40)), unchecked((int)(0x7f)) })
		{
		}

		protected internal ShortMessage(byte[] data) : base(data)
		{
		}

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.InvalidMidiDataException"></exception>
		public virtual void SetMessage(int status)
		{
			int dataLength = GetDataLength(status);
			if (dataLength != 0)
			{
				throw new InvalidMidiDataException("Status byte: " + status + " requires " + dataLength
					 + " data bytes length");
			}
			SetMessage(status, 0, 0);
		}

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.InvalidMidiDataException"></exception>
		public virtual void SetMessage(int status, int data1, int data2)
		{
			int dataLength = GetDataLength(status);
			if (dataLength > 0)
			{
				if (data1 < 0 || data1 > unchecked((int)(0x7f)))
				{
					throw new InvalidMidiDataException("data1 out of range: " + data1);
				}
				if (dataLength > 1)
				{
					if (data2 < 0 || data2 > unchecked((int)(0x7f)))
					{
						throw new InvalidMidiDataException("data2 out of range: " + data2);
					}
				}
			}
			if (data == null || data.Length < dataLength + 1)
			{
				data = new byte[dataLength + 1];
			}
			data[0] = unchecked((byte)(status & unchecked((int)(0xff))));
			if (data.Length > 1)
			{
				data[1] = unchecked((byte)(data1 & unchecked((int)(0xff))));
				if (data.Length > 2)
				{
					data[2] = unchecked((byte)(data2 & unchecked((int)(0xff))));
				}
			}
		}

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.InvalidMidiDataException"></exception>
		public virtual void SetMessage(int command, int channel, int data1, int data2)
		{
			if (command >= unchecked((int)(0xf0)) || command < unchecked((int)(0x80)))
			{
				throw new InvalidMidiDataException("command out of range: 0x" + Sharpen.Extensions.ToHexString
					(command));
			}
			if (channel > unchecked((int)(0x0f)))
			{
				throw new InvalidMidiDataException("channel out of range: " + channel);
			}
			SetMessage((command & unchecked((int)(0xf0))) | (channel & unchecked((int)(0x0f))
				), data1, data2);
		}

		public virtual int GetChannel()
		{
			return (GetStatus() & unchecked((int)(0x0f)));
		}

		public virtual int GetCommand()
		{
			return (GetStatus() & unchecked((int)(0xf0)));
		}

		public virtual int GetData1()
		{
			if (data.Length > 1)
			{
				return (data[1] & unchecked((int)(0xff)));
			}
			return 0;
		}

		public virtual int GetData2()
		{
			if (data.Length > 2)
			{
				return (data[2] & unchecked((int)(0xff)));
			}
			return 0;
		}

		public virtual object Clone()
		{
			byte[] result = new byte[data.Length];
			System.Array.Copy(data, 0, result, 0, result.Length);
			return new JP.KShoji.Javax.Sound.Midi.ShortMessage(result);
		}

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.InvalidMidiDataException"></exception>
		protected internal static int GetDataLength(int status)
		{
			switch (status)
			{
				case TUNE_REQUEST:
				case END_OF_EXCLUSIVE:
				case TIMING_CLOCK:
				case unchecked((int)(0xf9)):
				case START:
				case CONTINUE:
				case STOP:
				case unchecked((int)(0xfd)):
				case ACTIVE_SENSING:
				case SYSTEM_RESET:
				{
					return 0;
				}

				case MIDI_TIME_CODE:
				case SONG_SELECT:
				{
					return 1;
				}

				case SONG_POSITION_POINTER:
				{
					return 2;
				}

				default:
				{
					break;
				}
			}
			switch (status & unchecked((int)(0xf0)))
			{
				case NOTE_OFF:
				case NOTE_ON:
				case POLY_PRESSURE:
				case CONTROL_CHANGE:
				case PITCH_BEND:
				{
					return 2;
				}

				case PROGRAM_CHANGE:
				case CHANNEL_PRESSURE:
				{
					return 1;
				}

				default:
				{
					throw new InvalidMidiDataException("Invalid status byte: " + status);
				}
			}
		}
	}
}
