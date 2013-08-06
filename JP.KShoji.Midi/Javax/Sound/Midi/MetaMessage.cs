/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using JP.KShoji.Javax.Sound.Midi;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi
{
	public class MetaMessage : MidiMessage
	{
		public const int META = unchecked((int)(0xff));

		private static byte[] defaultMessage = new byte[] { unchecked((byte)META), 0 };

		private int dataLength = 0;

		public MetaMessage() : this(defaultMessage)
		{
		}

		/// <param name="data"></param>
		/// <exception cref="Sharpen.NegativeArraySizeException">
		/// MUST be caught. We can't throw
		/// <see cref="InvalidMidiDataException">InvalidMidiDataException</see>
		/// because of API compatibility.
		/// </exception>
		protected internal MetaMessage(byte[] data) : base(data)
		{
			if (data.Length >= 3)
			{
				dataLength = data.Length - 3;
				int pos = 2;
				while (pos < data.Length && (data[pos] & unchecked((int)(0x80))) != 0)
				{
					dataLength--;
					pos++;
				}
			}
			if (dataLength < 0)
			{
				// 'dataLength' may negative value. Negative 'dataLength' will throw NegativeArraySizeException when getData() called.
				throw new System.Exception("Invalid meta event. data: " + System.Text.Encoding.ASCII.GetString
					(data));
			}
		}

		/// <param name="type"></param>
		/// <param name="data"></param>
		/// <param name="length">unused parameter. Use always data.length</param>
		/// <exception cref="InvalidMidiDataException">InvalidMidiDataException</exception>
		/// <exception cref="JP.KShoji.Javax.Sound.Midi.InvalidMidiDataException"></exception>
		public virtual void SetMessage(int type, byte[] data, int length)
		{
			if (type >= 128 || type < 0)
			{
				throw new InvalidMidiDataException("Invalid meta event. type: " + type);
			}
			this.dataLength = data.Length;
			this.data = new byte[2 + GetMidiValuesLength(data.Length) + data.Length];
			this.data[0] = unchecked((byte)META);
			this.data[1] = unchecked((byte)type);
			WriteMidiValues(this.data, 2, data.Length);
			if (this.data.Length > 0)
			{
				System.Array.Copy(data, 0, this.data, this.data.Length - this.dataLength, this.dataLength
					);
			}
		}

		public virtual int GetType()
		{
			if (data.Length >= 2)
			{
				return data[1] & unchecked((int)(0xff));
			}
			return 0;
		}

		public virtual byte[] GetData()
		{
			byte[] returnedArray = new byte[dataLength];
			System.Array.Copy(data, (data.Length - dataLength), returnedArray, 0, dataLength);
			return returnedArray;
		}

		public override object Clone()
		{
			byte[] result = new byte[data.Length];
			System.Array.Copy(data, 0, result, 0, data.Length);
			return new JP.KShoji.Javax.Sound.Midi.MetaMessage(result);
		}

		private static int GetMidiValuesLength(long value)
		{
			int length = 0;
			long currentValue = value;
			do
			{
				currentValue = currentValue >> 7;
				length++;
			}
			while (currentValue > 0);
			return length;
		}

		private static void WriteMidiValues(byte[] data, int off, long value)
		{
			int shift = 63;
			while ((shift > 0) && ((value & (unchecked((int)(0x7f)) << shift)) == 0))
			{
				shift -= 7;
			}
			int currentOff = off;
			while (shift > 0)
			{
				data[currentOff++] = unchecked((byte)(((value & (unchecked((int)(0x7f)) << shift)
					) >> shift) | unchecked((int)(0x80))));
				shift -= 7;
			}
			data[currentOff] = unchecked((byte)(value & unchecked((int)(0x7f))));
		}
	}
}
