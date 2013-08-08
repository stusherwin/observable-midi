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
	/// <summary>Listener for MIDI detached events</summary>
	/// <author>K.Shoji</author>
	public interface OnMidiDeviceDetachedListener
	{
		/// <summary>device has been detached</summary>
		/// <param name="usbDevice"></param>
		void OnDeviceDetached(UsbDevice usbDevice);
	}
}
