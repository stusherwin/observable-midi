/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi
{
	[System.Serializable]
	public class MidiUnavailableException : Exception
	{
		private const long serialVersionUID = 6093809578628944323L;

		public MidiUnavailableException() : base()
		{
		}

		public MidiUnavailableException(string message) : base(message)
		{
		}
	}
}
