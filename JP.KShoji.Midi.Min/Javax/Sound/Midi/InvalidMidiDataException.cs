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
	public class InvalidMidiDataException : Exception
	{
		private const long serialVersionUID = 2780771756789932067L;

		public InvalidMidiDataException() : base()
		{
		}

		public InvalidMidiDataException(string message) : base(message)
		{
		}
	}
}
