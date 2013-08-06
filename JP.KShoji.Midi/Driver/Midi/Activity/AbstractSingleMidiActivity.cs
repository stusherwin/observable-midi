/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

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
	/// In this implement, the only one device (connected at first) will be detected.
	/// launchMode must be "singleTask" or "singleInstance".
	/// </remarks>
	/// <author>K.Shoji</author>
	public abstract class AbstractSingleMidiActivity : Android.App.Activity, OnMidiDeviceDetachedListener
		, OnMidiDeviceAttachedListener, OnMidiInputEventListener
	{
		/// <summary>Implementation for single device connections.</summary>
		/// <remarks>Implementation for single device connections.</remarks>
		/// <author>K.Shoji</author>
		internal sealed class OnMidiDeviceAttachedListenerImpl : OnMidiDeviceAttachedListener
		{
			private readonly UsbManager usbManager;

			/// <summary>constructor</summary>
			/// <param name="usbManager"></param>
			public OnMidiDeviceAttachedListenerImpl(AbstractSingleMidiActivity _enclosing, UsbManager
				 usbManager)
			{
				this._enclosing = _enclosing;
				this.usbManager = usbManager;
			}

			public void OnDeviceAttached(UsbDevice attachedDevice)
			{
				lock (this)
				{
					if (this._enclosing.device != null)
					{
						// already one device has been connected
						return;
					}
					this._enclosing.deviceConnection = this.usbManager.OpenDevice(attachedDevice);
					if (this._enclosing.deviceConnection == null)
					{
						return;
					}
					//IList<DeviceFilter> deviceFilters = DeviceFilter.GetDeviceFilters(this._enclosing
					//	.ApplicationContext);
                    IList<DeviceFilter> deviceFilters = new List<DeviceFilter>();
					ICollection<MidiInputDevice> foundInputDevices = UsbMidiDeviceUtils.FindMidiInputDevices
						(attachedDevice, this._enclosing.deviceConnection, deviceFilters, this._enclosing
						);
					if (foundInputDevices.Count > 0)
					{
						this._enclosing.midiInputDevice = (MidiInputDevice)Sharpen.Collections.ToArray(foundInputDevices
							)[0];
					}
					ICollection<MidiOutputDevice> foundOutputDevices = UsbMidiDeviceUtils.FindMidiOutputDevices
						(attachedDevice, this._enclosing.deviceConnection, deviceFilters);
					if (foundOutputDevices.Count > 0)
					{
						this._enclosing.midiOutputDevice = (MidiOutputDevice)Sharpen.Collections.ToArray(
							foundOutputDevices)[0];
					}
					Log.Debug(Constants.TAG, "Device " + attachedDevice.DeviceName + " has been attached."
						);
					this._enclosing.OnDeviceAttached(attachedDevice);
				}
			}

			private readonly AbstractSingleMidiActivity _enclosing;
		}

		/// <summary>Implementation for single device connections.</summary>
		/// <remarks>Implementation for single device connections.</remarks>
		/// <author>K.Shoji</author>
		internal sealed class OnMidiDeviceDetachedListenerImpl : OnMidiDeviceDetachedListener
		{
			public void OnDeviceDetached(UsbDevice detachedDevice)
			{
				lock (this)
				{
					if (this._enclosing.midiInputDevice != null)
					{
						this._enclosing.midiInputDevice.Stop();
						this._enclosing.midiInputDevice = null;
					}
					if (this._enclosing.midiOutputDevice != null)
					{
						this._enclosing.midiOutputDevice.Stop();
						this._enclosing.midiOutputDevice = null;
					}
					if (this._enclosing.deviceConnection != null)
					{
						this._enclosing.deviceConnection.Close();
						this._enclosing.deviceConnection = null;
					}
					this._enclosing.device = null;
					Log.Debug(Constants.TAG, "Device " + detachedDevice.DeviceName + " has been detached."
						);
					Message message = new Message();
                    message.Obj = detachedDevice;
					this._enclosing.deviceDetachedHandler.SendMessage(message);
				}
			}

			internal OnMidiDeviceDetachedListenerImpl(AbstractSingleMidiActivity _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly AbstractSingleMidiActivity _enclosing;
		}

		internal UsbDevice device = null;

		internal UsbDeviceConnection deviceConnection = null;

		internal MidiInputDevice midiInputDevice = null;

		internal MidiOutputDevice midiOutputDevice = null;

		internal OnMidiDeviceAttachedListener deviceAttachedListener = null;

		internal OnMidiDeviceDetachedListener deviceDetachedListener = null;

		internal Android.OS.Handler deviceDetachedHandler = null;

		private MidiDeviceConnectionWatcher deviceConnectionWatcher = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			UsbManager usbManager = (UsbManager)ApplicationContext.GetSystemService(Context
				.UsbService);
			deviceAttachedListener = new AbstractSingleMidiActivity.OnMidiDeviceAttachedListenerImpl
				(this, usbManager);
			deviceDetachedListener = new AbstractSingleMidiActivity.OnMidiDeviceDetachedListenerImpl
				(this);
			deviceDetachedHandler = new Android.OS.Handler(new _Callback_143(this));
			deviceConnectionWatcher = new MidiDeviceConnectionWatcher(ApplicationContext
				, usbManager, deviceAttachedListener, deviceDetachedListener);
		}

        private sealed class _Callback_143 : Android.OS.Handler.ICallback
		{
			public _Callback_143(AbstractSingleMidiActivity _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool HandleMessage(Message msg)
			{
                UsbDevice usbDevice = (UsbDevice)msg.Obj;
				this._enclosing.OnDeviceDetached(usbDevice);
				return true;
			}

			private readonly AbstractSingleMidiActivity _enclosing;

            #region IDisposable implementation

            public void Dispose()
            {
            }

            #endregion

            #region IJavaObject implementation

            public System.IntPtr Handle {
                get;
                set;
            }

            #endregion
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (deviceConnectionWatcher != null)
			{
				deviceConnectionWatcher.Stop();
			}
			deviceConnectionWatcher = null;
			if (midiInputDevice != null)
			{
				midiInputDevice.Stop();
				midiInputDevice = null;
			}
			midiOutputDevice = null;
			deviceConnection = null;
		}

		/// <summary>Get MIDI output device, if available.</summary>
		/// <remarks>Get MIDI output device, if available.</remarks>
		/// <param name="usbDevice"></param>
		/// <returns>MidiOutputDevice, null if not available</returns>
		public MidiOutputDevice GetMidiOutputDevice()
		{
			if (deviceConnectionWatcher != null)
			{
				deviceConnectionWatcher.CheckConnectedDevicesImmediately();
			}
			return midiOutputDevice;
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
