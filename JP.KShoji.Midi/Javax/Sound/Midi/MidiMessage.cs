/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi
{
	public abstract class MidiMessage : ICloneable
	{
		protected internal byte[] data;

		protected internal MidiMessage(byte[] data)
		{
			this.data = data;
		}

		/// <param name="data"></param>
		/// <param name="length">unused parameter. Use always data.length</param>
		/// <exception cref="InvalidMidiDataException">InvalidMidiDataException</exception>
		/// <exception cref="JP.KShoji.Javax.Sound.Midi.InvalidMidiDataException"></exception>
		protected internal virtual void SetMessage(byte[] data, int length)
		{
			if (this.data == null)
			{
				this.data = new byte[data.Length];
			}
			System.Array.Copy(data, 0, this.data, 0, data.Length);
		}

		public virtual byte[] GetMessage()
		{
			if (data == null)
			{
				return null;
			}
			byte[] resultArray = new byte[data.Length];
			System.Array.Copy(data, 0, resultArray, 0, data.Length);
			return resultArray;
		}

		public virtual int GetStatus()
		{
			if (data != null && data.Length > 0)
			{
				return (data[0] & unchecked((int)(0xff)));
			}
			return 0;
		}

		public virtual int GetLength()
		{
			if (data == null)
			{
				return 0;
			}
			return data.Length;
		}

        #region ICloneable implementation

        public abstract object Clone();

        #endregion
	}
}
