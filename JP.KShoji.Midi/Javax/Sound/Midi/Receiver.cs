/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using JP.KShoji.Javax.Sound.Midi;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi
{
	public interface Receiver
	{
		void Send(MidiMessage message, long timeStamp);

		void Close();
	}
}
