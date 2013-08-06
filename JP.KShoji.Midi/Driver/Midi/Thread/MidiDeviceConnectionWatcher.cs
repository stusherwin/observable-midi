/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Util;
using JP.KShoji.Driver.Midi.Listener;
using JP.KShoji.Driver.Midi.Thread;
using JP.KShoji.Driver.Midi.Util;
using JP.KShoji.Driver.Usb.Util;
using Sharpen;

namespace JP.KShoji.Driver.Midi.Thread
{
	/// <summary>
	/// Detects USB MIDI Device Connected
	/// stop() method must be called when the application will be destroyed.
	/// </summary>
	/// <remarks>
	/// Detects USB MIDI Device Connected
	/// stop() method must be called when the application will be destroyed.
	/// </remarks>
	/// <author>K.Shoji</author>
	public sealed class MidiDeviceConnectionWatcher
	{
		private readonly MidiDeviceConnectionWatcher.MidiDeviceConnectionWatchThread thread;

		internal readonly Dictionary<string, UsbDevice> grantedDeviceMap;

		internal readonly Queue<UsbDevice> deviceGrantQueue;

		internal volatile bool isGranting;

		/// <summary>constructor</summary>
		/// <param name="context"></param>
		/// <param name="usbManager"></param>
		/// <param name="deviceAttachedListener"></param>
		/// <param name="deviceDetachedListener"></param>
		public MidiDeviceConnectionWatcher(Context context, UsbManager usbManager, OnMidiDeviceAttachedListener
			 deviceAttachedListener, OnMidiDeviceDetachedListener deviceDetachedListener)
		{
			deviceGrantQueue = new Queue<UsbDevice>();
			isGranting = false;
			grantedDeviceMap = new Dictionary<string, UsbDevice>();
			thread = new MidiDeviceConnectionWatcher.MidiDeviceConnectionWatchThread(this, context
				, usbManager, deviceAttachedListener, deviceDetachedListener);
			thread.Start();
		}

		public void CheckConnectedDevicesImmediately()
		{
			thread.CheckConnectedDevices();
		}

		/// <summary>
		/// stops the watching thread <br />
		/// <br />
		/// Note: Takes one second until the thread stops.
		/// </summary>
		/// <remarks>
		/// stops the watching thread <br />
		/// <br />
		/// Note: Takes one second until the thread stops.
		/// The device attached / detached events will be noticed until the thread will completely stops.
		/// </remarks>
		public void Stop()
		{
			thread.stopFlag = true;
			// blocks while the thread will stop
			while (thread.IsAlive())
			{
				try
				{
					Sharpen.Thread.Sleep(100);
				}
				catch (Exception)
				{
				}
			}
		}

		/// <summary>Broadcast receiver for MIDI device connection granted</summary>
		/// <author>K.Shoji</author>
		private sealed class UsbMidiGrantedReceiver : BroadcastReceiver
		{
			public static readonly string USB_PERMISSION_GRANTED_ACTION = "jp.kshoji.driver.midi.USB_PERMISSION_GRANTED_ACTION";

			private readonly string deviceName;

			private readonly UsbDevice device;

			private readonly OnMidiDeviceAttachedListener deviceAttachedListener;

			/// <param name="device"></param>
			/// <param name="deviceAttachedListener"></param>
			public UsbMidiGrantedReceiver(MidiDeviceConnectionWatcher _enclosing, string deviceName
				, UsbDevice device, OnMidiDeviceAttachedListener deviceAttachedListener)
			{
				this._enclosing = _enclosing;
				// ignore
				this.deviceName = deviceName;
				this.device = device;
				this.deviceAttachedListener = deviceAttachedListener;
			}

			public override void OnReceive(Context context, Intent intent)
			{
				string action = intent.Action;
				if (MidiDeviceConnectionWatcher.UsbMidiGrantedReceiver.USB_PERMISSION_GRANTED_ACTION
					.Equals(action))
				{
					bool granted = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
					if (granted)
					{
						if (this.deviceAttachedListener != null && this.device != null)
						{
							this._enclosing.grantedDeviceMap.Put(this.deviceName, this.device);
							this.deviceAttachedListener.OnDeviceAttached(this.device);
						}
					}
				}
			}

			private readonly MidiDeviceConnectionWatcher _enclosing;
		}

		/// <summary>USB Device polling thread</summary>
		/// <author>K.Shoji</author>
		private sealed class MidiDeviceConnectionWatchThread : Sharpen.Thread
		{
			private Context context;

			private UsbManager usbManager;

			private OnMidiDeviceAttachedListener deviceAttachedListener;

			private OnMidiDeviceDetachedListener deviceDetachedListener;

			private ICollection<string> connectedDeviceNameSet;

			private ICollection<string> removedDeviceNames;

			internal bool stopFlag;

			private IList<DeviceFilter> deviceFilters;

			/// <summary>constructor</summary>
			/// <param name="context"></param>
			/// <param name="usbManager"></param>
			/// <param name="deviceAttachedListener"></param>
			/// <param name="deviceDetachedListener"></param>
			internal MidiDeviceConnectionWatchThread(MidiDeviceConnectionWatcher _enclosing, 
				Context context, UsbManager usbManager, OnMidiDeviceAttachedListener deviceAttachedListener
				, OnMidiDeviceDetachedListener deviceDetachedListener)
			{
				this._enclosing = _enclosing;
				this.context = context;
				this.usbManager = usbManager;
				this.deviceAttachedListener = deviceAttachedListener;
				this.deviceDetachedListener = deviceDetachedListener;
				this.connectedDeviceNameSet = new HashSet<string>();
				this.removedDeviceNames = new HashSet<string>();
				this.stopFlag = false;
                this.deviceFilters = new List<DeviceFilter>(); //DeviceFilter.GetDeviceFilters(context);
			}

			public override void Run()
			{
				base.Run();
				while (this.stopFlag == false)
				{
					this.CheckConnectedDevices();
					lock (this._enclosing.deviceGrantQueue)
					{
						if (this._enclosing.deviceGrantQueue.Count != 0 && !this._enclosing.isGranting)
						{
							this._enclosing.isGranting = true;
							UsbDevice device = this._enclosing.deviceGrantQueue.Dequeue();
							PendingIntent permissionIntent = PendingIntent.GetBroadcast(this.context, 0, new 
								Intent(MidiDeviceConnectionWatcher.UsbMidiGrantedReceiver.USB_PERMISSION_GRANTED_ACTION
								), 0);
							this.context.RegisterReceiver(new MidiDeviceConnectionWatcher.UsbMidiGrantedReceiver
                                                          (this._enclosing, device.DeviceName, device, this.deviceAttachedListener), new IntentFilter
								(MidiDeviceConnectionWatcher.UsbMidiGrantedReceiver.USB_PERMISSION_GRANTED_ACTION
								));
							this.usbManager.RequestPermission(device, permissionIntent);
						}
					}
					try
					{
						Sharpen.Thread.Sleep(1000);
					}
					catch (Exception e)
					{
						Log.Debug(Constants.TAG, "Thread interrupted", e);
					}
				}
			}

			/// <summary>checks Attached/Detached devices</summary>
			internal void CheckConnectedDevices()
			{
				lock (this)
				{
					IDictionary<string, UsbDevice> deviceMap = this.usbManager.DeviceList;
					// check attached device
					foreach (string deviceName in deviceMap.Keys)
					{
						// check if already removed
						if (this.removedDeviceNames.Contains(deviceName))
						{
							continue;
						}
						if (!this.connectedDeviceNameSet.Contains(deviceName))
						{
							this.connectedDeviceNameSet.AddItem(deviceName);
							UsbDevice device = deviceMap.Get(deviceName);
							ICollection<UsbInterface> midiInterfaces = UsbMidiDeviceUtils.FindAllMidiInterfaces
								(device, this.deviceFilters);
							if (midiInterfaces.Count > 0)
							{
								lock (this._enclosing.deviceGrantQueue)
								{
									this._enclosing.deviceGrantQueue.Enqueue(device);
								}
							}
						}
					}
					// check detached device
					foreach (string deviceName_1 in this.connectedDeviceNameSet)
					{
						if (!deviceMap.ContainsKey(deviceName_1))
						{
							this.removedDeviceNames.AddItem(deviceName_1);
							UsbDevice device = Sharpen.Collections.Remove(this._enclosing.grantedDeviceMap, deviceName_1
								);
							Log.Debug(Constants.TAG, "deviceName:" + deviceName_1 + ", device:" + device + " detached."
								);
							if (device != null)
							{
								this.deviceDetachedListener.OnDeviceDetached(device);
							}
						}
					}
					this.connectedDeviceNameSet.RemoveAll(this.removedDeviceNames);
				}
			}

			private readonly MidiDeviceConnectionWatcher _enclosing;
		}

		/// <summary>notifies the 'current granting device' has successfully granted.</summary>
		/// <remarks>notifies the 'current granting device' has successfully granted.</remarks>
		public void NotifyDeviceGranted()
		{
			isGranting = false;
		}
	}
}
