/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using JP.KShoji.Driver.Midi.Device;
using JP.KShoji.Driver.Midi.Listener;
using Sharpen;

namespace JP.KShoji.Driver.Midi.Listener
{
	/// <summary>
	/// Listener for MIDI events
	/// For events' details, @see <a href="http://www.usb.org/developers/devclass_docs/midi10.pdf">Universal Serial Bus Device Class Definition for MIDI Devices</a>
	/// </summary>
	/// <author>K.Shoji</author>
	public interface OnMidiInputEventListener
	{
		/// <summary>Miscellaneous function codes.</summary>
		/// <remarks>
		/// Miscellaneous function codes. Reserved for future extensions.
		/// Code Index Number : 0x0
		/// </remarks>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="byte1"></param>
		/// <param name="byte2"></param>
		/// <param name="byte3"></param>
		void OnMidiMiscellaneousFunctionCodes(MidiInputDevice sender, int cable, int byte1
			, int byte2, int byte3);

		/// <summary>Cable events.</summary>
		/// <remarks>
		/// Cable events. Reserved for future expansion.
		/// Code Index Number : 0x1
		/// </remarks>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="byte1"></param>
		/// <param name="byte2"></param>
		/// <param name="byte3"></param>
		void OnMidiCableEvents(MidiInputDevice sender, int cable, int byte1, int byte2, int
			 byte3);

		/// <summary>System Common messages, or SysEx ends with following single byte.</summary>
		/// <remarks>
		/// System Common messages, or SysEx ends with following single byte.
		/// Code Index Number : 0x2 0x3 0x5
		/// </remarks>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="bytes">bytes.length:1, 2, or 3</param>
		void OnMidiSystemCommonMessage(MidiInputDevice sender, int cable, byte[] bytes);

		/// <summary>
		/// SysEx
		/// Code Index Number : 0x4, 0x5, 0x6, 0x7
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="systemExclusive"></param>
		void OnMidiSystemExclusive(MidiInputDevice sender, int cable, byte[] systemExclusive
			);

		/// <summary>
		/// Note-off
		/// Code Index Number : 0x8
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="note">0-127</param>
		/// <param name="velocity">0-127</param>
		void OnMidiNoteOff(MidiInputDevice sender, int cable, int channel, int note, int 
			velocity);

		/// <summary>
		/// Note-on
		/// Code Index Number : 0x9
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="note">0-127</param>
		/// <param name="velocity">0-127</param>
		void OnMidiNoteOn(MidiInputDevice sender, int cable, int channel, int note, int velocity
			);

		/// <summary>
		/// Poly-KeyPress
		/// Code Index Number : 0xa
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="note">0-127</param>
		/// <param name="pressure">0-127</param>
		void OnMidiPolyphonicAftertouch(MidiInputDevice sender, int cable, int channel, int
			 note, int pressure);

		/// <summary>
		/// Control Change
		/// Code Index Number : 0xb
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="function">0-127</param>
		/// <param name="value">0-127</param>
		void OnMidiControlChange(MidiInputDevice sender, int cable, int channel, int function
			, int value);

		/// <summary>
		/// Program Change
		/// Code Index Number : 0xc
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="program">0-127</param>
		void OnMidiProgramChange(MidiInputDevice sender, int cable, int channel, int program
			);

		/// <summary>
		/// Channel Pressure
		/// Code Index Number : 0xd
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="pressure">0-127</param>
		void OnMidiChannelAftertouch(MidiInputDevice sender, int cable, int channel, int 
			pressure);

		/// <summary>
		/// PitchBend Change
		/// Code Index Number : 0xe
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="amount">0(low)-8192(center)-16383(high)</param>
		void OnMidiPitchWheel(MidiInputDevice sender, int cable, int channel, int amount);

		/// <summary>
		/// Single Byte
		/// Code Index Number : 0xf
		/// </summary>
		/// <param name="senderDevice"></param>
		/// <param name="senderInterface"></param>
		/// <param name="cable">0-15</param>
		/// <param name="byte1"></param>
		void OnMidiSingleByte(MidiInputDevice sender, int cable, int byte1);

		/// <summary>RPN message</summary>
		/// <param name="sender"></param>
		/// <param name="cable"></param>
		/// <param name="channel"></param>
		/// <param name="function">14bits</param>
		/// <param name="valueMSB">higher 7bits</param>
		/// <param name="valueLSB">lower 7bits. -1 if value has no LSB. If you know the function's parameter value have LSB, you must ignore when valueLSB &lt; 0.
		/// 	</param>
		void OnMidiRPNReceived(MidiInputDevice sender, int cable, int channel, int function
			, int valueMSB, int valueLSB);

		/// <summary>NRPN message</summary>
		/// <param name="sender"></param>
		/// <param name="cable"></param>
		/// <param name="channel"></param>
		/// <param name="function">14bits</param>
		/// <param name="valueMSB">higher 7bits</param>
		/// <param name="valueLSB">lower 7bits. -1 if value has no LSB. If you know the function's parameter value have LSB, you must ignore when valueLSB &lt; 0.
		/// 	</param>
		void OnMidiNRPNReceived(MidiInputDevice sender, int cable, int channel, int function
			, int valueMSB, int valueLSB);
	}
}
