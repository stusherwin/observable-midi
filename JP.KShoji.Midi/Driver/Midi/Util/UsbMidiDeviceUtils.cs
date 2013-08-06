/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System.Collections.Generic;
using Android.Hardware.Usb;
using Android.Util;
using JP.KShoji.Driver.Midi.Device;
using JP.KShoji.Driver.Midi.Listener;
using JP.KShoji.Driver.Midi.Util;
using JP.KShoji.Driver.Usb.Util;
using Sharpen;

namespace JP.KShoji.Driver.Midi.Util
{
	/// <summary>Utility for finding MIDI device</summary>
	/// <author>K.Shoji</author>
	public sealed class UsbMidiDeviceUtils
	{
		/// <summary>
		/// Find
		/// <see cref="Android.Hardware.Usb.UsbInterface">Android.Hardware.Usb.UsbInterface</see>
		/// from
		/// <see cref="Android.Hardware.Usb.UsbDevice">Android.Hardware.Usb.UsbDevice</see>
		/// with the direction
		/// </summary>
		/// <param name="usbDevice"></param>
		/// <param name="direction">
		/// 
		/// <see cref="UsbConstants.USB_DIR_IN">UsbConstants.USB_DIR_IN</see>
		/// or
		/// <see cref="UsbConstants.USB_DIR_OUT">UsbConstants.USB_DIR_OUT</see>
		/// </param>
		/// <param name="deviceFilters"></param>
		/// <returns>
		/// 
		/// <see>Set<UsbInterface></see>
		/// always not null
		/// </returns>
		public static ICollection<UsbInterface> FindMidiInterfaces(UsbDevice usbDevice, int
			 direction, IList<DeviceFilter> deviceFilters)
		{
			ICollection<UsbInterface> usbInterfaces = new HashSet<UsbInterface>();
			int count = usbDevice.InterfaceCount;
			for (int i = 0; i < count; i++)
			{
				UsbInterface usbInterface = usbDevice.GetInterface(i);
				if (FindMidiEndpoint(usbDevice, usbInterface, direction, deviceFilters) != null)
				{
					usbInterfaces.AddItem(usbInterface);
				}
			}
			return Sharpen.Collections.UnmodifiableSet(usbInterfaces);
		}

		/// <summary>
		/// Find all
		/// <see cref="Android.Hardware.Usb.UsbInterface">Android.Hardware.Usb.UsbInterface</see>
		/// from
		/// <see cref="Android.Hardware.Usb.UsbDevice">Android.Hardware.Usb.UsbDevice</see>
		/// </summary>
		/// <param name="usbDevice"></param>
		/// <param name="deviceFilters"></param>
		/// <returns>
		/// 
		/// <see>Set<UsbInterface></see>
		/// always not null
		/// </returns>
		public static ICollection<UsbInterface> FindAllMidiInterfaces(UsbDevice usbDevice
			, IList<DeviceFilter> deviceFilters)
		{
			ICollection<UsbInterface> usbInterfaces = new HashSet<UsbInterface>();
			int count = usbDevice.InterfaceCount;
			for (int i = 0; i < count; i++)
			{
				UsbInterface usbInterface = usbDevice.GetInterface(i);
				if (FindMidiEndpoint(usbDevice, usbInterface, (int)UsbAddressing.In, deviceFilters
					) != null)
				{
					usbInterfaces.AddItem(usbInterface);
				}
                if (FindMidiEndpoint(usbDevice, usbInterface, (int)UsbAddressing.Out, deviceFilters
					) != null)
				{
					usbInterfaces.AddItem(usbInterface);
				}
			}
			return Sharpen.Collections.UnmodifiableSet(usbInterfaces);
		}

		/// <summary>
		/// Find
		/// <see>Set<MidiIntputDevice></see>
		/// from
		/// <see cref="Android.Hardware.Usb.UsbDevice">Android.Hardware.Usb.UsbDevice</see>
		/// </summary>
		/// <param name="usbDevice"></param>
		/// <param name="usbDeviceConnection"></param>
		/// <param name="deviceFilters"></param>
		/// <param name="inputEventListener"></param>
		/// <returns>
		/// 
		/// <see>Set<MidiIntputDevice></see>
		/// always not null
		/// </returns>
		public static ICollection<MidiInputDevice> FindMidiInputDevices(UsbDevice usbDevice
			, UsbDeviceConnection usbDeviceConnection, IList<DeviceFilter> deviceFilters, OnMidiInputEventListener
			 inputEventListener)
		{
			ICollection<MidiInputDevice> devices = new HashSet<MidiInputDevice>();
			int count = usbDevice.InterfaceCount;
			for (int i = 0; i < count; i++)
			{
				UsbInterface usbInterface = usbDevice.GetInterface(i);
				UsbEndpoint endpoint = FindMidiEndpoint(usbDevice, usbInterface, (int)UsbAddressing.In
					, deviceFilters);
				if (endpoint != null)
				{
					devices.AddItem(new MidiInputDevice(usbDevice, usbDeviceConnection, usbInterface, 
						endpoint, inputEventListener));
				}
			}
			return Sharpen.Collections.UnmodifiableSet(devices);
		}

		/// <summary>
		/// Find
		/// <see>Set<MidiOutputDevice></see>
		/// from
		/// <see cref="Android.Hardware.Usb.UsbDevice">Android.Hardware.Usb.UsbDevice</see>
		/// </summary>
		/// <param name="usbDevice"></param>
		/// <param name="usbDeviceConnection"></param>
		/// <param name="deviceFilters"></param>
		/// <returns>
		/// 
		/// <see>Set<MidiOutputDevice></see>
		/// always not null
		/// </returns>
		public static ICollection<MidiOutputDevice> FindMidiOutputDevices(UsbDevice usbDevice
			, UsbDeviceConnection usbDeviceConnection, IList<DeviceFilter> deviceFilters)
		{
			ICollection<MidiOutputDevice> devices = new HashSet<MidiOutputDevice>();
			int count = usbDevice.InterfaceCount;
			for (int i = 0; i < count; i++)
			{
				UsbInterface usbInterface = usbDevice.GetInterface(i);
				if (usbInterface == null)
				{
					continue;
				}
				UsbEndpoint endpoint = FindMidiEndpoint(usbDevice, usbInterface, (int)UsbAddressing.Out
					, deviceFilters);
				if (endpoint != null)
				{
					devices.AddItem(new MidiOutputDevice(usbDevice, usbDeviceConnection, usbInterface
						, endpoint));
				}
			}
			return Sharpen.Collections.UnmodifiableSet(devices);
		}

		/// <summary>
		/// Find
		/// <see cref="Android.Hardware.Usb.UsbEndpoint">Android.Hardware.Usb.UsbEndpoint</see>
		/// from
		/// <see cref="findMidiEndpoint">findMidiEndpoint</see>
		/// with the direction
		/// </summary>
		/// <param name="usbDevice"></param>
		/// <param name="usbInterface"></param>
		/// <param name="direction"></param>
		/// <param name="deviceFilters"></param>
		/// <returns>
		/// 
		/// <see cref="Android.Hardware.Usb.UsbEndpoint">Android.Hardware.Usb.UsbEndpoint</see>
		/// , null if not found
		/// </returns>
		public static UsbEndpoint FindMidiEndpoint(UsbDevice usbDevice, UsbInterface usbInterface
			, int direction, IList<DeviceFilter> deviceFilters)
		{
			int endpointCount = usbInterface.EndpointCount;
			// standard USB MIDI interface
            if ((int)usbInterface.InterfaceClass == 1 && (int)usbInterface.InterfaceSubclass 
				== 3)
			{
				for (int endpointIndex = 0; endpointIndex < endpointCount; endpointIndex++)
				{
					UsbEndpoint endpoint = usbInterface.GetEndpoint(endpointIndex);
                    if ((int)endpoint.Direction == direction)
					{
						return endpoint;
					}
				}
			}
			else
			{
				bool filterMatched = false;
				foreach (DeviceFilter deviceFilter in deviceFilters)
				{
					if (deviceFilter.Matches(usbDevice))
					{
						filterMatched = true;
						break;
					}
				}
				if (filterMatched == false)
				{
					Log.Debug(Constants.TAG, "unsupported interface: " + usbInterface);
					return null;
				}
				// non standard USB MIDI interface
				for (int endpointIndex = 0; endpointIndex < endpointCount; endpointIndex++)
				{
					UsbEndpoint endpoint = usbInterface.GetEndpoint(endpointIndex);
					if ((endpoint.Type == UsbAddressing.XferBulk || endpoint.Type
						 == UsbAddressing.XferInterrupt))
					{
                        if ((int)endpoint.Direction == direction)
						{
							return endpoint;
						}
					}
				}
			}
			return null;
		}
	}
}
