/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using JP.KShoji.Javax.Sound.Midi;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi
{
	public class SysexMessage : MidiMessage
	{
		public const int SYSTEM_EXCLUSIVE = unchecked((int)(0xf0));

		public const int SPECIAL_SYSTEM_EXCLUSIVE = unchecked((int)(0xf7));

		public SysexMessage() : this(new byte[] { unchecked((byte)(SYSTEM_EXCLUSIVE & unchecked(
			(int)(0xff)))), unchecked((byte)(ShortMessage.END_OF_EXCLUSIVE & unchecked((int)
			(0xff)))) })
		{
		}

		protected internal SysexMessage(byte[] data) : base(data)
		{
		}

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.InvalidMidiDataException"></exception>
		protected internal override void SetMessage(byte[] data, int length)
		{
			int status = (data[0] & unchecked((int)(0xff)));
			if ((status != SYSTEM_EXCLUSIVE) && (status != SPECIAL_SYSTEM_EXCLUSIVE))
			{
				throw new InvalidMidiDataException("Invalid status byte for SysexMessage: 0x" + Sharpen.Extensions.ToHexString
					(status));
			}
			base.SetMessage(data, length);
		}

		/// <param name="status">must be SYSTEM_EXCLUSIVE or SPECIAL_SYSTEM_EXCLUSIVE</param>
		/// <param name="data"></param>
		/// <param name="length">unused parameter. Use always data.length</param>
		/// <exception cref="InvalidMidiDataException">InvalidMidiDataException</exception>
		/// <exception cref="JP.KShoji.Javax.Sound.Midi.InvalidMidiDataException"></exception>
		public virtual void SetMessage(int status, byte[] data, int length)
		{
			if ((status != SYSTEM_EXCLUSIVE) && (status != SPECIAL_SYSTEM_EXCLUSIVE))
			{
				throw new InvalidMidiDataException("Invalid status byte for SysexMessage: 0x" + Sharpen.Extensions.ToHexString
					(status));
			}
			if (this.data == null || this.data.Length < data.Length + 1)
			{
				this.data = new byte[data.Length + 1];
			}
			this.data[0] = unchecked((byte)(status & unchecked((int)(0xff))));
			if (data.Length > 0)
			{
				System.Array.Copy(data, 0, this.data, 1, data.Length);
			}
		}

		public virtual byte[] GetData()
		{
			byte[] returnedArray = new byte[data.Length - 1];
			System.Array.Copy(data, 1, returnedArray, 0, (data.Length - 1));
			return returnedArray;
		}

		public override object Clone()
		{
			byte[] result = new byte[data.Length];
			System.Array.Copy(data, 0, result, 0, result.Length);
			return new JP.KShoji.Javax.Sound.Midi.SysexMessage(result);
		}
	}
}
