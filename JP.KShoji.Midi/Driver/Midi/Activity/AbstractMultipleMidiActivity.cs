/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Util;
using JP.KShoji.Driver.Midi.Activity;
using JP.KShoji.Driver.Midi.Device;
using JP.KShoji.Driver.Midi.Listener;
using JP.KShoji.Driver.Midi.Thread;
using JP.KShoji.Driver.Midi.Util;
using JP.KShoji.Driver.Usb.Util;
using Sharpen;

namespace JP.KShoji.Driver.Midi.Activity
{
	/// <summary>base Activity for using USB MIDI interface.</summary>
	/// <remarks>
	/// base Activity for using USB MIDI interface.
	/// In this implement, each devices will be detected on connect.
	/// launchMode must be "singleTask" or "singleInstance".
	/// </remarks>
	/// <author>K.Shoji</author>
	public abstract class AbstractMultipleMidiActivity : Android.App.Activity, OnMidiDeviceDetachedListener
		, OnMidiDeviceAttachedListener, OnMidiInputEventListener
	{
		/// <summary>Implementation for multiple device connections.</summary>
		/// <remarks>Implementation for multiple device connections.</remarks>
		/// <author>K.Shoji</author>
		internal sealed class OnMidiDeviceAttachedListenerImpl : OnMidiDeviceAttachedListener
		{
			private readonly UsbManager usbManager;

			/// <summary>constructor</summary>
			/// <param name="usbManager"></param>
			public OnMidiDeviceAttachedListenerImpl(AbstractMultipleMidiActivity _enclosing, 
				UsbManager usbManager)
			{
				this._enclosing = _enclosing;
				this.usbManager = usbManager;
			}

			public void OnDeviceAttached(UsbDevice attachedDevice)
			{
				lock (this)
				{
					// these fields are null; when this event fired while Activity destroying.
					if (this._enclosing.midiInputDevices == null || this._enclosing.midiOutputDevices
						 == null || this._enclosing.deviceConnections == null)
					{
						// nothing to do
						return;
					}
					this._enclosing.deviceConnectionWatcher.NotifyDeviceGranted();
					UsbDeviceConnection deviceConnection = this.usbManager.OpenDevice(attachedDevice);
					if (deviceConnection == null)
					{
						return;
					}
					this._enclosing.deviceConnections.Put(attachedDevice, deviceConnection);
					//IList<DeviceFilter> deviceFilters = DeviceFilter.GetDeviceFilters(this._enclosing
					//	.ApplicationContext);
                    IList<DeviceFilter> deviceFilters = new List<DeviceFilter>();
					ICollection<MidiInputDevice> foundInputDevices = UsbMidiDeviceUtils.FindMidiInputDevices
						(attachedDevice, deviceConnection, deviceFilters, this._enclosing);
					foreach (MidiInputDevice midiInputDevice in foundInputDevices)
					{
						try
						{
							ICollection<MidiInputDevice> inputDevices = this._enclosing.midiInputDevices.Get(
								attachedDevice);
							if (inputDevices == null)
							{
								inputDevices = new HashSet<MidiInputDevice>();
							}
							inputDevices.AddItem(midiInputDevice);
							this._enclosing.midiInputDevices.Put(attachedDevice, inputDevices);
						}
						catch (ArgumentException iae)
						{
							Log.Info(Constants.TAG, "This device didn't have any input endpoints.", iae);
						}
					}
					ICollection<MidiOutputDevice> foundOutputDevices = UsbMidiDeviceUtils.FindMidiOutputDevices
						(attachedDevice, deviceConnection, deviceFilters);
					foreach (MidiOutputDevice midiOutputDevice in foundOutputDevices)
					{
						try
						{
							ICollection<MidiOutputDevice> outputDevices = this._enclosing.midiOutputDevices.Get
								(attachedDevice);
							if (outputDevices == null)
							{
								outputDevices = new HashSet<MidiOutputDevice>();
							}
							outputDevices.AddItem(midiOutputDevice);
							this._enclosing.midiOutputDevices.Put(attachedDevice, outputDevices);
						}
						catch (ArgumentException iae)
						{
							Log.Info(Constants.TAG, "This device didn't have any output endpoints.", iae);
						}
					}
					Log.Debug(Constants.TAG, "Device " + attachedDevice.DeviceName + " has been attached."
						);
					this._enclosing.OnDeviceAttached(attachedDevice);
				}
			}

			private readonly AbstractMultipleMidiActivity _enclosing;
		}

		/// <summary>Implementation for multiple device connections.</summary>
		/// <remarks>Implementation for multiple device connections.</remarks>
		/// <author>K.Shoji</author>
		internal sealed class OnMidiDeviceDetachedListenerImpl : OnMidiDeviceDetachedListener
		{
			public void OnDeviceDetached(UsbDevice detachedDevice)
			{
				lock (this)
				{
					// these fields are null; when this event fired while Activity destroying.
					if (this._enclosing.midiInputDevices == null || this._enclosing.midiOutputDevices
						 == null || this._enclosing.deviceConnections == null)
					{
						// nothing to do
						return;
					}
					// Stop input device's thread.
					ICollection<MidiInputDevice> inputDevices = this._enclosing.midiInputDevices.Get(
						detachedDevice);
					if (inputDevices != null && inputDevices.Count > 0)
					{
						foreach (MidiInputDevice inputDevice in inputDevices)
						{
							if (inputDevice != null)
							{
								inputDevice.Stop();
							}
						}
						Sharpen.Collections.Remove(this._enclosing.midiInputDevices, detachedDevice);
					}
					ICollection<MidiOutputDevice> outputDevices = this._enclosing.midiOutputDevices.Get
						(detachedDevice);
					if (outputDevices != null)
					{
						foreach (MidiOutputDevice outputDevice in outputDevices)
						{
							if (outputDevice != null)
							{
								outputDevice.Stop();
							}
						}
						Sharpen.Collections.Remove(this._enclosing.midiOutputDevices, detachedDevice);
					}
					UsbDeviceConnection deviceConnection = this._enclosing.deviceConnections.Get(detachedDevice
						);
					if (deviceConnection != null)
					{
						deviceConnection.Close();
						Sharpen.Collections.Remove(this._enclosing.deviceConnections, detachedDevice);
					}
					Log.Debug(Constants.TAG, "Device " + detachedDevice.DeviceName + " has been detached."
						);
					Message message = new Message();
					message.Obj = detachedDevice;
					this._enclosing.deviceDetachedHandler.SendMessage(message);
				}
			}

			internal OnMidiDeviceDetachedListenerImpl(AbstractMultipleMidiActivity _enclosing
				)
			{
				this._enclosing = _enclosing;
			}

			private readonly AbstractMultipleMidiActivity _enclosing;
		}

		internal IDictionary<UsbDevice, UsbDeviceConnection> deviceConnections = null;

		internal IDictionary<UsbDevice, ICollection<MidiInputDevice>> midiInputDevices = 
			null;

		internal IDictionary<UsbDevice, ICollection<MidiOutputDevice>> midiOutputDevices = 
			null;

		internal OnMidiDeviceAttachedListener deviceAttachedListener = null;

		internal OnMidiDeviceDetachedListener deviceDetachedListener = null;

		internal Android.OS.Handler deviceDetachedHandler = null;

		internal MidiDeviceConnectionWatcher deviceConnectionWatcher = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			deviceConnections = new Dictionary<UsbDevice, UsbDeviceConnection>();
			midiInputDevices = new Dictionary<UsbDevice, ICollection<MidiInputDevice>>();
			midiOutputDevices = new Dictionary<UsbDevice, ICollection<MidiOutputDevice>>();
			UsbManager usbManager = (UsbManager)ApplicationContext.GetSystemService(Context
				.UsbService);
			deviceAttachedListener = new AbstractMultipleMidiActivity.OnMidiDeviceAttachedListenerImpl
				(this, usbManager);
			deviceDetachedListener = new AbstractMultipleMidiActivity.OnMidiDeviceDetachedListenerImpl
				(this);
			deviceDetachedHandler = new Android.OS.Handler(new _Callback_190(this));
			deviceConnectionWatcher = new MidiDeviceConnectionWatcher(ApplicationContext
				, usbManager, deviceAttachedListener, deviceDetachedListener);
		}

        private sealed class _Callback_190 : Android.OS.Handler.ICallback
		{
			public _Callback_190(AbstractMultipleMidiActivity _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool HandleMessage(Message msg)
			{
                Log.Info(Constants.TAG, "(handleMessage) detached device:" + msg.Obj);
                UsbDevice usbDevice = (UsbDevice)msg.Obj;
                this._enclosing.OnDeviceDetached(usbDevice);
				return true;
			}

			private readonly AbstractMultipleMidiActivity _enclosing;

            #region IDisposable implementation

            public void Dispose()
            {
            }

            #endregion

            #region IJavaObject implementation

            public IntPtr Handle {
                get;
                set;
            }

            #endregion
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			deviceConnectionWatcher.Stop();
			deviceConnectionWatcher = null;
			if (midiInputDevices != null)
			{
				foreach (ICollection<MidiInputDevice> inputDevices in midiInputDevices.Values)
				{
					if (inputDevices != null)
					{
						foreach (MidiInputDevice inputDevice in inputDevices)
						{
							if (inputDevice != null)
							{
								inputDevice.Stop();
							}
						}
					}
				}
				midiInputDevices.Clear();
			}
			midiInputDevices = null;
			if (midiOutputDevices != null)
			{
				midiOutputDevices.Clear();
			}
			midiOutputDevices = null;
			deviceConnections = null;
		}

		/// <summary>Get connected USB MIDI devices.</summary>
		/// <remarks>Get connected USB MIDI devices.</remarks>
		/// <returns>connected UsbDevice set</returns>
		public ICollection<UsbDevice> GetConnectedUsbDevices()
		{
			if (deviceConnectionWatcher != null)
			{
				deviceConnectionWatcher.CheckConnectedDevicesImmediately();
			}
			if (deviceConnections != null)
			{
				return Sharpen.Collections.UnmodifiableSet(deviceConnections.Keys);
			}
			return Sharpen.Collections.UnmodifiableSet(new HashSet<UsbDevice>());
		}

		/// <summary>Get MIDI output device, if available.</summary>
		/// <remarks>Get MIDI output device, if available.</remarks>
		/// <param name="usbDevice"></param>
		/// <returns>
		/// 
		/// <see>Set<MidiOutputDevice></see>
		/// </returns>
		public ICollection<MidiOutputDevice> GetMidiOutputDevices(UsbDevice usbDevice)
		{
			if (deviceConnectionWatcher != null)
			{
				deviceConnectionWatcher.CheckConnectedDevicesImmediately();
			}
			if (midiOutputDevices != null)
			{
				return Sharpen.Collections.UnmodifiableSet(midiOutputDevices.Get(usbDevice));
			}
			return Sharpen.Collections.UnmodifiableSet(new HashSet<MidiOutputDevice>());
		}

		/// <summary>
		/// RPN message
		/// This method is just the utility method, do not need to be implemented necessarily by subclass.
		/// </summary>
		/// <remarks>
		/// RPN message
		/// This method is just the utility method, do not need to be implemented necessarily by subclass.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="cable"></param>
		/// <param name="channel"></param>
		/// <param name="function">14bits</param>
		/// <param name="valueMSB">higher 7bits</param>
		/// <param name="valueLSB">lower 7bits. -1 if value has no LSB. If you know the function's parameter value have LSB, you must ignore when valueLSB &lt; 0.
		/// 	</param>
		public virtual void OnMidiRPNReceived(MidiInputDevice sender, int cable, int channel
			, int function, int valueMSB, int valueLSB)
		{
		}

		// do nothing in this implementation
		/// <summary>
		/// NRPN message
		/// This method is just the utility method, do not need to be implemented necessarily by subclass.
		/// </summary>
		/// <remarks>
		/// NRPN message
		/// This method is just the utility method, do not need to be implemented necessarily by subclass.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="cable"></param>
		/// <param name="channel"></param>
		/// <param name="function">14bits</param>
		/// <param name="valueMSB">higher 7bits</param>
		/// <param name="valueLSB">lower 7bits. -1 if value has no LSB. If you know the function's parameter value have LSB, you must ignore when valueLSB &lt; 0.
		/// 	</param>
		public virtual void OnMidiNRPNReceived(MidiInputDevice sender, int cable, int channel
			, int function, int valueMSB, int valueLSB)
		{
		}

		public abstract void OnDeviceDetached(UsbDevice arg1);

		public abstract void OnDeviceAttached(UsbDevice arg1);

		public abstract void OnMidiCableEvents(MidiInputDevice arg1, int arg2, int arg3, 
			int arg4, int arg5);

		public abstract void OnMidiChannelAftertouch(MidiInputDevice arg1, int arg2, int 
			arg3, int arg4);

		public abstract void OnMidiControlChange(MidiInputDevice arg1, int arg2, int arg3
			, int arg4, int arg5);

		public abstract void OnMidiMiscellaneousFunctionCodes(MidiInputDevice arg1, int arg2
			, int arg3, int arg4, int arg5);

		public abstract void OnMidiNoteOff(MidiInputDevice arg1, int arg2, int arg3, int 
			arg4, int arg5);

		public abstract void OnMidiNoteOn(MidiInputDevice arg1, int arg2, int arg3, int arg4
			, int arg5);

		public abstract void OnMidiPitchWheel(MidiInputDevice arg1, int arg2, int arg3, int
			 arg4);

		public abstract void OnMidiPolyphonicAftertouch(MidiInputDevice arg1, int arg2, int
			 arg3, int arg4, int arg5);

		public abstract void OnMidiProgramChange(MidiInputDevice arg1, int arg2, int arg3
			, int arg4);

		public abstract void OnMidiSingleByte(MidiInputDevice arg1, int arg2, int arg3);

		public abstract void OnMidiSystemCommonMessage(MidiInputDevice arg1, int arg2, byte
			[] arg3);

		public abstract void OnMidiSystemExclusive(MidiInputDevice arg1, int arg2, byte[]
			 arg3);
		// do nothing in this implementation
	}
}
