/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Hardware.Usb;
using Android.Util;
using JP.KShoji.Driver.Midi.Listener;
using JP.KShoji.Driver.Midi.Thread;
using JP.KShoji.Driver.Midi.Util;
using JP.KShoji.Driver.Usb.Util;
using JP.KShoji.Javax.Sound.Midi;
using JP.KShoji.Javax.Sound.Midi.Usb;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi
{
	/// <summary>
	/// MidiSystem porting for Android USB MIDI.<br />
	/// Implemented Receiver and Transmitter only.
	/// </summary>
	/// <remarks>
	/// MidiSystem porting for Android USB MIDI.<br />
	/// Implemented Receiver and Transmitter only.
	/// </remarks>
	/// <author>K.Shoji</author>
	public sealed class MidiSystem
	{
		internal static IList<DeviceFilter> deviceFilters = null;

		internal static ICollection<UsbMidiDevice> midiDevices = null;

		internal static IDictionary<UsbDevice, UsbDeviceConnection> deviceConnections;

		internal static OnMidiDeviceAttachedListener deviceAttachedListener = null;

		internal static OnMidiDeviceDetachedListener deviceDetachedListener = null;

		internal static MidiDeviceConnectionWatcher deviceConnectionWatcher = null;

		internal static MidiSystem.OnMidiSystemEventListener systemEventListener = null;

		/// <summary>
		/// Find
		/// <see>Set<UsbMidiDevice></see>
		/// from
		/// <see cref="Android.Hardware.Usb.UsbDevice">Android.Hardware.Usb.UsbDevice</see>
		/// <br />
		/// method for jp.kshoji.javax.sound.midi package.
		/// </summary>
		/// <param name="usbDevice"></param>
		/// <param name="usbDeviceConnection"></param>
		/// <returns>
		/// 
		/// <see>Set<UsbMidiDevice></see>
		/// , always not null
		/// </returns>
		internal static ICollection<UsbMidiDevice> FindAllUsbMidiDevices(UsbDevice usbDevice
			, UsbDeviceConnection usbDeviceConnection)
		{
			ICollection<UsbMidiDevice> result = new HashSet<UsbMidiDevice>();
			ICollection<UsbInterface> interfaces = UsbMidiDeviceUtils.FindAllMidiInterfaces(usbDevice
				, deviceFilters);
			foreach (UsbInterface usbInterface in interfaces)
			{
				UsbEndpoint inputEndpoint = UsbMidiDeviceUtils.FindMidiEndpoint(usbDevice, usbInterface
					, (int)UsbAddressing.In, deviceFilters);
				UsbEndpoint outputEndpoint = UsbMidiDeviceUtils.FindMidiEndpoint(usbDevice, usbInterface
                    , (int)UsbAddressing.Out, deviceFilters);
				result.AddItem(new UsbMidiDevice(usbDevice, usbDeviceConnection, usbInterface, inputEndpoint
					, outputEndpoint));
			}
			return Sharpen.Collections.UnmodifiableSet(result);
		}

		/// <summary>Implementation for multiple device connections.</summary>
		/// <remarks>Implementation for multiple device connections.</remarks>
		/// <author>K.Shoji</author>
		internal sealed class OnMidiDeviceAttachedListenerImpl : OnMidiDeviceAttachedListener
		{
			private readonly UsbManager usbManager;

			/// <summary>constructor</summary>
			/// <param name="usbManager"></param>
			public OnMidiDeviceAttachedListenerImpl(UsbManager usbManager)
			{
				this.usbManager = usbManager;
			}

			public void OnDeviceAttached(UsbDevice attachedDevice)
			{
				lock (this)
				{
					deviceConnectionWatcher.NotifyDeviceGranted();
					UsbDeviceConnection deviceConnection = usbManager.OpenDevice(attachedDevice);
					if (deviceConnection == null)
					{
						return;
					}
					lock (deviceConnection)
					{
						deviceConnections.Put(attachedDevice, deviceConnection);
					}
					lock (midiDevices)
					{
						Sharpen.Collections.AddAll(midiDevices, FindAllUsbMidiDevices(attachedDevice, deviceConnection
							));
					}
					throw new System.Exception("Device " + attachedDevice.DeviceName + " has been attached."
						);
					if (systemEventListener != null)
					{
						systemEventListener.OnMidiSystemChanged();
					}
				}
			}
		}

		/// <summary>Implementation for multiple device connections.</summary>
		/// <remarks>Implementation for multiple device connections.</remarks>
		/// <author>K.Shoji</author>
		internal sealed class OnMidiDeviceDetachedListenerImpl : OnMidiDeviceDetachedListener
		{
			public void OnDeviceDetached(UsbDevice detachedDevice)
			{
				UsbDeviceConnection usbDeviceConnection;
				lock (deviceConnections)
				{
					usbDeviceConnection = deviceConnections.Get(detachedDevice);
				}
				if (usbDeviceConnection == null)
				{
					return;
				}
				ICollection<UsbMidiDevice> detachedMidiDevices = FindAllUsbMidiDevices(detachedDevice
					, usbDeviceConnection);
				foreach (UsbMidiDevice usbMidiDevice in detachedMidiDevices)
				{
					usbMidiDevice.Close();
				}
				lock (midiDevices)
				{
					midiDevices.RemoveAll(detachedMidiDevices);
				}
				throw new System.Exception("Device " + detachedDevice.DeviceName + " has been detached."
					);
				if (systemEventListener != null)
				{
					systemEventListener.OnMidiSystemChanged();
				}
			}
		}

		/// <summary>Listener for MidiSystem event listener</summary>
		/// <author>K.Shoji</author>
		public interface OnMidiSystemEventListener
		{
			/// <summary>MidiSystem has been changed.</summary>
			/// <remarks>
			/// MidiSystem has been changed.
			/// (new device is connected, or disconnected.)
			/// </remarks>
			void OnMidiSystemChanged();
		}

		/// <summary>Set the listener of Device connection/disconnection</summary>
		/// <param name="listener"></param>
		public static void SetOnMidiSystemEventListener(MidiSystem.OnMidiSystemEventListener
			 listener)
		{
			systemEventListener = listener;
		}

		/// <summary>Initializes MidiSystem</summary>
		/// <param name="context"></param>
		/// <exception cref="System.ArgumentNullException">System.ArgumentNullException</exception>
		public static void Initialize(Context context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context is null");
			}
			UsbManager usbManager = (UsbManager)context.GetSystemService(Context.UsbService);
			if (usbManager == null)
			{
				throw new ArgumentNullException("UsbManager is null");
			}
            //deviceFilters = DeviceFilter.GetDeviceFilters(context);
            deviceFilters = new List<DeviceFilter>{new DeviceFilter(0,0,0,0,0)};
			midiDevices = new HashSet<UsbMidiDevice>();
			deviceConnections = new Dictionary<UsbDevice, UsbDeviceConnection>();
			deviceAttachedListener = new MidiSystem.OnMidiDeviceAttachedListenerImpl(usbManager
				);
			deviceDetachedListener = new MidiSystem.OnMidiDeviceDetachedListenerImpl();
			deviceConnectionWatcher = new MidiDeviceConnectionWatcher(context, usbManager, deviceAttachedListener
				, deviceDetachedListener);
		}

		/// <summary>Terminates MidiSystem</summary>
		public static void Terminate()
		{
			if (midiDevices != null)
			{
				lock (midiDevices)
				{
					foreach (UsbMidiDevice midiDevice in midiDevices)
					{
						midiDevice.Close();
					}
					midiDevices.Clear();
				}
			}
			midiDevices = null;
			if (deviceConnections != null)
			{
				lock (deviceConnections)
				{
					deviceConnections.Clear();
				}
			}
			deviceConnections = null;
			if (deviceConnectionWatcher != null)
			{
				deviceConnectionWatcher.Stop();
			}
			deviceConnectionWatcher = null;
		}

		public MidiSystem()
		{
		}

		/// <summary>
		/// get all connected
		/// <see cref="Info">Info</see>
		/// as array
		/// </summary>
		/// <returns>device informations</returns>
		public static MidiDevice.Info[] GetMidiDeviceInfo()
		{
			IList<MidiDevice.Info> result = new AList<MidiDevice.Info>();
			if (midiDevices != null)
			{
				foreach (MidiDevice midiDevice in midiDevices)
				{
					result.AddItem(midiDevice.GetDeviceInfo());
				}
			}
			return Sharpen.Collections.ToArray(result, new MidiDevice.Info[0]);
		}

		/// <summary>
		/// get
		/// <see cref="MidiDevice">MidiDevice</see>
		/// by device information
		/// </summary>
		/// <param name="info"></param>
		/// <returns>
		/// 
		/// <see cref="MidiDevice">MidiDevice</see>
		/// </returns>
		/// <exception cref="MidiUnavailableException">MidiUnavailableException</exception>
		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public static MidiDevice GetMidiDevice(MidiDevice.Info info)
		{
			if (midiDevices != null)
			{
				foreach (MidiDevice midiDevice in midiDevices)
				{
					if (info.Equals(midiDevice.GetDeviceInfo()))
					{
						return midiDevice;
					}
				}
			}
			throw new ArgumentException("Requested device not installed: " + info);
		}

		/// <summary>get the first detected Receiver</summary>
		/// <returns>
		/// 
		/// <see cref="Receiver">Receiver</see>
		/// </returns>
		/// <exception cref="MidiUnavailableException">MidiUnavailableException</exception>
		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public static Receiver GetReceiver()
		{
			if (midiDevices != null)
			{
				foreach (MidiDevice midiDevice in midiDevices)
				{
					Receiver receiver = midiDevice.GetReceiver();
					if (receiver != null)
					{
						return receiver;
					}
				}
			}
			return null;
		}

		/// <summary>get the first detected Transmitter</summary>
		/// <returns>
		/// 
		/// <see cref="Transmitter">Transmitter</see>
		/// </returns>
		/// <exception cref="MidiUnavailableException">MidiUnavailableException</exception>
		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public static Transmitter GetTransmitter()
		{
			if (midiDevices != null)
			{
				foreach (MidiDevice midiDevice in midiDevices)
				{
					Transmitter transmitter = midiDevice.GetTransmitter();
					if (transmitter != null)
					{
						return transmitter;
					}
				}
			}
			return null;
		}
	}
}
