/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System;
using Android.Hardware.Usb;
using JP.KShoji.Driver.Midi.Device;
using JP.KShoji.Javax.Sound.Midi;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi.Usb
{
	/// <summary>
	/// <see cref="JP.KShoji.Javax.Sound.Midi.Receiver">JP.KShoji.Javax.Sound.Midi.Receiver
	/// 	</see>
	/// implementation
	/// </summary>
	/// <author>K.Shoji</author>
	public sealed class UsbMidiReceiver : Receiver
	{
		private readonly UsbDevice usbDevice;

		private readonly UsbDeviceConnection usbDeviceConnection;

		private readonly UsbInterface usbInterface;

		private readonly UsbEndpoint outputEndpoint;

		private int cableId;

		private MidiOutputDevice outputDevice = null;

		public UsbMidiReceiver(UsbDevice usbDevice, UsbDeviceConnection usbDeviceConnection
			, UsbInterface usbInterface, UsbEndpoint outputEndpoint)
		{
			this.usbDevice = usbDevice;
			this.usbDeviceConnection = usbDeviceConnection;
			this.usbInterface = usbInterface;
			this.outputEndpoint = outputEndpoint;
			cableId = 0;
		}

		public void Send(MidiMessage message, long timeStamp)
		{
			if (outputDevice == null)
			{
				throw new InvalidOperationException("Receiver not opened.");
			}
			if (message is MetaMessage)
			{
				MetaMessage metaMessage = (MetaMessage)message;
				outputDevice.SendMidiSystemCommonMessage(cableId, metaMessage.GetData());
			}
			else
			{
				if (message is SysexMessage)
				{
					SysexMessage sysexMessage = (SysexMessage)message;
					outputDevice.SendMidiSystemExclusive(cableId, sysexMessage.GetData());
				}
				else
				{
					if (message is ShortMessage)
					{
						ShortMessage shortMessage = (ShortMessage)message;
						switch (shortMessage.GetCommand())
						{
							case ShortMessage.CHANNEL_PRESSURE:
							{
								outputDevice.SendMidiChannelAftertouch(cableId, shortMessage.GetChannel(), shortMessage
									.GetData1());
								break;
							}

							case ShortMessage.CONTROL_CHANGE:
							{
								outputDevice.SendMidiControlChange(cableId, shortMessage.GetChannel(), shortMessage
									.GetData1(), shortMessage.GetData2());
								break;
							}

							case ShortMessage.NOTE_OFF:
							{
								outputDevice.SendMidiNoteOff(cableId, shortMessage.GetChannel(), shortMessage.GetData1
									(), shortMessage.GetData2());
								break;
							}

							case ShortMessage.NOTE_ON:
							{
								outputDevice.SendMidiNoteOn(cableId, shortMessage.GetChannel(), shortMessage.GetData1
									(), shortMessage.GetData2());
								break;
							}

							case ShortMessage.PITCH_BEND:
							{
								outputDevice.SendMidiPitchWheel(cableId, shortMessage.GetChannel(), shortMessage.
									GetData1() | (shortMessage.GetData2() << 7));
								break;
							}

							case ShortMessage.POLY_PRESSURE:
							{
								outputDevice.SendMidiPolyphonicAftertouch(cableId, shortMessage.GetChannel(), shortMessage
									.GetData1(), shortMessage.GetData2());
								break;
							}

							case ShortMessage.PROGRAM_CHANGE:
							{
								outputDevice.SendMidiProgramChange(cableId, shortMessage.GetChannel(), shortMessage
									.GetData1());
								break;
							}

							default:
							{
								break;
							}
						}
					}
				}
			}
		}

		public void Open()
		{
			outputDevice = new MidiOutputDevice(usbDevice, usbDeviceConnection, usbInterface, 
				outputEndpoint);
		}

		public void Close()
		{
			if (outputDevice != null)
			{
				outputDevice.Stop();
			}
		}

		public int GetCableId()
		{
			return cableId;
		}

		public void SetCableId(int cableId)
		{
			this.cableId = cableId;
		}
	}
}
