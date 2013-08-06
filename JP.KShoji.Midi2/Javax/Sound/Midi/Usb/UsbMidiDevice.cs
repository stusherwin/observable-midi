/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System.Collections.Generic;
using Android.Hardware.Usb;
using JP.KShoji.Javax.Sound.Midi;
using JP.KShoji.Javax.Sound.Midi.Usb;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi.Usb
{
	/// <summary>
	/// <see cref="JP.KShoji.Javax.Sound.Midi.MidiDevice">JP.KShoji.Javax.Sound.Midi.MidiDevice
	/// 	</see>
	/// implementation
	/// </summary>
	/// <author>K.Shoji</author>
	public sealed class UsbMidiDevice : MidiDevice
	{
		private readonly UsbDevice usbDevice;

		private readonly UsbDeviceConnection usbDeviceConnection;

		private readonly UsbInterface usbInterface;

		private readonly IList<Receiver> receivers = new AList<Receiver>();

		private readonly IList<Transmitter> transmitters = new AList<Transmitter>();

		private bool isOpened;

		public UsbMidiDevice(UsbDevice usbDevice, UsbDeviceConnection usbDeviceConnection
			, UsbInterface usbInterface, UsbEndpoint inputEndpoint, UsbEndpoint outputEndpoint
			)
		{
			this.usbDevice = usbDevice;
			this.usbDeviceConnection = usbDeviceConnection;
			this.usbInterface = usbInterface;
			receivers.AddItem(new UsbMidiReceiver(usbDevice, usbDeviceConnection, usbInterface
				, outputEndpoint));
			transmitters.AddItem(new UsbMidiTransmitter(usbDevice, usbDeviceConnection, usbInterface
				, inputEndpoint));
			isOpened = false;
		}

		public override MidiDevice.Info GetDeviceInfo()
		{
			return new MidiDevice.Info(usbDevice.GetDeviceName(), string.Format("vendorId: %x, productId: %x"
				, usbDevice.GetVendorId(), usbDevice.GetProductId()), "deviceId:" + usbDevice.GetDeviceId
				(), "interfaceId:" + usbInterface.GetId());
		}

		//
		//
		//
		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public override void Open()
		{
			if (isOpened)
			{
				return;
			}
			foreach (Receiver receiver in receivers)
			{
				if (receiver is UsbMidiReceiver)
				{
					UsbMidiReceiver usbMidiReceiver = (UsbMidiReceiver)receiver;
					// claimInterface will be called
					usbMidiReceiver.Open();
				}
			}
			foreach (Transmitter transmitter in transmitters)
			{
				if (transmitter is UsbMidiTransmitter)
				{
					UsbMidiTransmitter usbMidiTransmitter = (UsbMidiTransmitter)transmitter;
					// claimInterface will be called
					usbMidiTransmitter.Open();
				}
			}
			isOpened = true;
		}

		public override void Close()
		{
			if (!isOpened)
			{
				return;
			}
			foreach (Transmitter transmitter in transmitters)
			{
				transmitter.Close();
			}
			transmitters.Clear();
			foreach (Receiver receiver in receivers)
			{
				receiver.Close();
			}
			receivers.Clear();
			if (usbDeviceConnection != null && usbInterface != null)
			{
				usbDeviceConnection.ReleaseInterface(usbInterface);
			}
			isOpened = false;
		}

		public override bool IsOpen()
		{
			return isOpened;
		}

		public override long GetMicrosecondPosition()
		{
			// time-stamping is not supported
			return -1;
		}

		public override int GetMaxReceivers()
		{
			if (receivers != null)
			{
				return receivers.Count;
			}
			return 0;
		}

		public override int GetMaxTransmitters()
		{
			if (transmitters != null)
			{
				return transmitters.Count;
			}
			return 0;
		}

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public override Receiver GetReceiver()
		{
			if (receivers == null || receivers.Count < 1)
			{
				return null;
			}
			return receivers[0];
		}

		public override IList<Receiver> GetReceivers()
		{
			return Sharpen.Collections.UnmodifiableList(receivers);
		}

		/// <exception cref="JP.KShoji.Javax.Sound.Midi.MidiUnavailableException"></exception>
		public override Transmitter GetTransmitter()
		{
			if (transmitters == null || transmitters.Count < 1)
			{
				return null;
			}
			return transmitters[0];
		}

		public override IList<Transmitter> GetTransmitters()
		{
			return Sharpen.Collections.UnmodifiableList(transmitters);
		}
	}
}
