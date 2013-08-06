/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System;
using Android.Hardware.Usb;
using Android.OS;
using Android.Util;
using JP.KShoji.Driver.Midi.Device;
using JP.KShoji.Driver.Midi.Handler;
using JP.KShoji.Driver.Midi.Listener;
using JP.KShoji.Driver.Midi.Util;
using Sharpen;

namespace JP.KShoji.Driver.Midi.Device
{
	/// <summary>
	/// MIDI Input Device
	/// stop() method must be called when the application will be destroyed.
	/// </summary>
	/// <remarks>
	/// MIDI Input Device
	/// stop() method must be called when the application will be destroyed.
	/// </remarks>
	/// <author>K.Shoji</author>
	public sealed class MidiInputDevice
	{
		private readonly UsbDevice usbDevice;

		internal readonly UsbDeviceConnection usbDeviceConnection;

		private readonly UsbInterface usbInterface;

		internal readonly UsbEndpoint inputEndpoint;

		private readonly MidiInputDevice.WaiterThread waiterThread;

		/// <summary>constructor</summary>
		/// <param name="usbDevice"></param>
		/// <param name="usbDeviceConnection"></param>
		/// <param name="usbInterface"></param>
		/// <param name="midiEventListener"></param>
		/// <exception cref="System.ArgumentException">endpoint not found.</exception>
		public MidiInputDevice(UsbDevice usbDevice, UsbDeviceConnection usbDeviceConnection
			, UsbInterface usbInterface, UsbEndpoint usbEndpoint, OnMidiInputEventListener midiEventListener
			)
		{
			this.usbDevice = usbDevice;
			this.usbDeviceConnection = usbDeviceConnection;
			this.usbInterface = usbInterface;
			waiterThread = new MidiInputDevice.WaiterThread(this, new Android.OS.Handler(new MidiMessageCallback
				(this, midiEventListener)));
			inputEndpoint = usbEndpoint;
			if (inputEndpoint == null)
			{
				throw new ArgumentException("Input endpoint was not found.");
			}
			usbDeviceConnection.ClaimInterface(usbInterface, true);
			waiterThread.Start();
		}

		/// <summary>stops the watching thread</summary>
		public void Stop()
		{
			waiterThread.stopFlag = true;
			// blocks while the thread will stop
			while (waiterThread.IsAlive())
			{
				try
				{
					Sharpen.Thread.Sleep(100);
				}
				catch (Exception)
				{
				}
			}
			// ignore
			usbDeviceConnection.ReleaseInterface(usbInterface);
		}

		/// <returns>the usbDevice</returns>
		public UsbDevice GetUsbDevice()
		{
			return usbDevice;
		}

		/// <returns>the usbInterface</returns>
		public UsbInterface GetUsbInterface()
		{
			return usbInterface;
		}

		/// <returns>the usbEndpoint</returns>
		public UsbEndpoint GetUsbEndpoint()
		{
			return inputEndpoint;
		}

		/// <summary>Polling thread for input data.</summary>
		/// <remarks>
		/// Polling thread for input data.
		/// Loops infinitely while stopFlag == false.
		/// </remarks>
		/// <author>K.Shoji</author>
		private sealed class WaiterThread : Sharpen.Thread
		{
			private byte[] readBuffer = new byte[64];

			internal bool stopFlag;

			private Android.OS.Handler receiveHandler;

			/// <summary>constructor</summary>
			/// <param name="handler"></param>
			internal WaiterThread(MidiInputDevice _enclosing, Android.OS.Handler handler)
			{
				this._enclosing = _enclosing;
				this.stopFlag = false;
				this.receiveHandler = handler;
			}

			public override void Run()
			{
				while (true)
				{
					if (this.stopFlag)
					{
						return;
					}
					if (this._enclosing.inputEndpoint == null)
					{
						continue;
					}
					int length = this._enclosing.usbDeviceConnection.BulkTransfer(this._enclosing.inputEndpoint
						, this.readBuffer, this.readBuffer.Length, 0);
					if (length > 0)
					{
						byte[] read = new byte[length];
						System.Array.Copy(this.readBuffer, 0, read, 0, length);
						throw new System.Exception("Input:" + System.Text.ASCIIEncoding.ASCII.GetString(read));
						Message message = new Message();
                        message.Obj = read;
						if (!this.stopFlag)
						{
							this.receiveHandler.SendMessage(message);
						}
					}
				}
			}

			private readonly MidiInputDevice _enclosing;
		}
	}
}
