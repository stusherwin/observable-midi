/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using Android.Hardware.Usb;
using JP.KShoji.Driver.Midi.Listener;
using Sharpen;

namespace JP.KShoji.Driver.Midi.Listener
{
	/// <summary>Listener for MIDI attached events</summary>
	/// <author>K.Shoji</author>
	public interface OnMidiDeviceAttachedListener
	{
		/// <summary>device has been attached</summary>
		/// <param name="usbDevice"></param>
		void OnDeviceAttached(UsbDevice usbDevice);
	}
}
