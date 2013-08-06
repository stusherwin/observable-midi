/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using System;
using System.IO;
using Android.Hardware.Usb;
using Android.Util;
using JP.KShoji.Driver.Midi.Util;
using Sharpen;

namespace JP.KShoji.Driver.Midi.Device
{
	/// <summary>MIDI Output Device</summary>
	/// <author>K.Shoji</author>
	public sealed class MidiOutputDevice
	{
		private readonly UsbDevice usbDevice;

		private readonly UsbInterface usbInterface;

		private readonly UsbDeviceConnection deviceConnection;

		private readonly UsbEndpoint outputEndpoint;

		private UsbRequest usbRequest;

		/// <summary>constructor</summary>
		/// <param name="usbDevice"></param>
		/// <param name="usbDeviceConnection"></param>
		/// <param name="usbInterface"></param>
		public MidiOutputDevice(UsbDevice usbDevice, UsbDeviceConnection usbDeviceConnection
			, UsbInterface usbInterface, UsbEndpoint usbEndpoint)
		{
			this.usbDevice = usbDevice;
			this.deviceConnection = usbDeviceConnection;
			this.usbInterface = usbInterface;
			outputEndpoint = usbEndpoint;
			if (outputEndpoint == null)
			{
				throw new ArgumentException("Output endpoint was not found.");
			}
			Log.Info(Constants.TAG, "deviceConnection:" + deviceConnection + ", usbInterface:" +
				 usbInterface);
			deviceConnection.ClaimInterface(this.usbInterface, true);
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
			return outputEndpoint;
		}

		/// <summary>stop to use this device.</summary>
		/// <remarks>stop to use this device.</remarks>
		public void Stop()
		{
			if (usbRequest != null)
			{
				usbRequest.Close();
			}
			deviceConnection.ReleaseInterface(usbInterface);
		}

		/// <summary>Sends MIDI message to output device.</summary>
		/// <remarks>Sends MIDI message to output device.</remarks>
		/// <param name="codeIndexNumber"></param>
		/// <param name="cable"></param>
		/// <param name="byte1"></param>
		/// <param name="byte2"></param>
		/// <param name="byte3"></param>
		private void SendMidiMessage(int codeIndexNumber, int cable, int byte1, int byte2
			, int byte3)
		{
			byte[] writeBuffer = new byte[4];
			writeBuffer[0] = unchecked((byte)(((cable & unchecked((int)(0xf))) << 4) | (codeIndexNumber
				 & unchecked((int)(0xf)))));
			writeBuffer[1] = unchecked((byte)byte1);
			writeBuffer[2] = unchecked((byte)byte2);
			writeBuffer[3] = unchecked((byte)byte3);
			// usbRequest.queue() is not thread-safe
			lock (deviceConnection)
			{
				if (usbRequest == null)
				{
					usbRequest = new UsbRequest();
					usbRequest.Initialize(deviceConnection, outputEndpoint);
				}
				while (usbRequest.Queue(Java.Nio.ByteBuffer.Wrap(writeBuffer), 4) == false)
				{
					// loop until queue completed
					try
					{
						Sharpen.Thread.Sleep(1);
					}
					catch (Exception)
					{
					}
				}
				// ignore exception
				while (usbRequest.Equals(deviceConnection.RequestWait()) == false)
				{
					// loop until result received
					try
					{
						Sharpen.Thread.Sleep(1);
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// ignore exception
		/// <summary>Miscellaneous function codes.</summary>
		/// <remarks>
		/// Miscellaneous function codes. Reserved for future extensions.
		/// Code Index Number : 0x0
		/// </remarks>
		/// <param name="cable">0-15</param>
		/// <param name="byte1"></param>
		/// <param name="byte2"></param>
		/// <param name="byte3"></param>
		public void SendMidiMiscellaneousFunctionCodes(int cable, int byte1, int byte2, int
			 byte3)
		{
			SendMidiMessage(unchecked((int)(0x0)), cable, byte1, byte2, byte3);
		}

		/// <summary>Cable events.</summary>
		/// <remarks>
		/// Cable events. Reserved for future expansion.
		/// Code Index Number : 0x1
		/// </remarks>
		/// <param name="cable">0-15</param>
		/// <param name="byte1"></param>
		/// <param name="byte2"></param>
		/// <param name="byte3"></param>
		public void SendMidiCableEvents(int cable, int byte1, int byte2, int byte3)
		{
			SendMidiMessage(unchecked((int)(0x1)), cable, byte1, byte2, byte3);
		}

		/// <summary>System Common messages, or SysEx ends with following single byte.</summary>
		/// <remarks>
		/// System Common messages, or SysEx ends with following single byte.
		/// Code Index Number : 0x2 0x3 0x5
		/// </remarks>
		/// <param name="cable">0-15</param>
		/// <param name="bytes">bytes.length:1, 2, or 3</param>
		public void SendMidiSystemCommonMessage(int cable, byte[] bytes)
		{
			if (bytes == null)
			{
				return;
			}
			switch (bytes.Length)
			{
				case 1:
				{
					SendMidiMessage(unchecked((int)(0x5)), cable, bytes[0] & unchecked((int)(0xff)), 
						0, 0);
					break;
				}

				case 2:
				{
					SendMidiMessage(unchecked((int)(0x2)), cable, bytes[0] & unchecked((int)(0xff)), 
						bytes[1] & unchecked((int)(0xff)), 0);
					break;
				}

				case 3:
				{
					SendMidiMessage(unchecked((int)(0x3)), cable, bytes[0] & unchecked((int)(0xff)), 
						bytes[1] & unchecked((int)(0xff)), bytes[2] & unchecked((int)(0xff)));
					break;
				}

				default:
				{
					// do nothing.
					break;
					break;
				}
			}
		}

		/// <summary>
		/// SysEx
		/// Code Index Number : 0x4, 0x5, 0x6, 0x7
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="systemExclusive">: start with 'F0', and end with 'F7'</param>
		public void SendMidiSystemExclusive(int cable, byte[] systemExclusive)
		{
			ByteArrayOutputStream transferDataStream = new ByteArrayOutputStream();
			for (int sysexIndex = 0; sysexIndex < systemExclusive.Length; sysexIndex += 3)
			{
				if ((sysexIndex + 3 < systemExclusive.Length))
				{
					// sysex starts or continues...
					transferDataStream.Write((((cable & unchecked((int)(0xf))) << 4) | unchecked((int
						)(0x4))));
					transferDataStream.Write(systemExclusive[sysexIndex + 0] & unchecked((int)(0xff))
						);
					transferDataStream.Write(systemExclusive[sysexIndex + 1] & unchecked((int)(0xff))
						);
					transferDataStream.Write(systemExclusive[sysexIndex + 2] & unchecked((int)(0xff))
						);
				}
				else
				{
					switch (systemExclusive.Length % 3)
					{
						case 1:
						{
							// sysex end with 1 byte
							transferDataStream.Write((((cable & unchecked((int)(0xf))) << 4) | unchecked((int
								)(0x5))));
							transferDataStream.Write(systemExclusive[sysexIndex + 0] & unchecked((int)(0xff))
								);
							transferDataStream.Write(0);
							transferDataStream.Write(0);
							break;
						}

						case 2:
						{
							// sysex end with 2 bytes
							transferDataStream.Write((((cable & unchecked((int)(0xf))) << 4) | unchecked((int
								)(0x6))));
							transferDataStream.Write(systemExclusive[sysexIndex + 0] & unchecked((int)(0xff))
								);
							transferDataStream.Write(systemExclusive[sysexIndex + 1] & unchecked((int)(0xff))
								);
							transferDataStream.Write(0);
							break;
						}

						case 0:
						{
							// sysex end with 3 bytes
							transferDataStream.Write((((cable & unchecked((int)(0xf))) << 4) | unchecked((int
								)(0x7))));
							transferDataStream.Write(systemExclusive[sysexIndex + 0] & unchecked((int)(0xff))
								);
							transferDataStream.Write(systemExclusive[sysexIndex + 1] & unchecked((int)(0xff))
								);
							transferDataStream.Write(systemExclusive[sysexIndex + 2] & unchecked((int)(0xff))
								);
							break;
						}
					}
				}
			}
			byte[] buffer = transferDataStream.ToByteArray();
			// usbRequest.queue() is not thread-safe
			lock (deviceConnection)
			{
				if (usbRequest == null)
				{
					usbRequest = new UsbRequest();
					usbRequest.Initialize(deviceConnection, outputEndpoint);
				}
                while (usbRequest.Queue(Java.Nio.ByteBuffer.Wrap(buffer), buffer.Length) == false)
				{
					// loop until queue completed
					try
					{
						Sharpen.Thread.Sleep(1);
					}
					catch (Exception)
					{
					}
				}
				// ignore exception
				while (usbRequest.Equals(deviceConnection.RequestWait()) == false)
				{
					// loop until result received
					try
					{
						Sharpen.Thread.Sleep(1);
					}
					catch (Exception)
					{
					}
				}
			}
			// ignore exception
			throw new System.Exception(string.Empty + buffer.Length + " bytes of " + buffer.Length 
				+ " bytes has been queued for transfering.");
		}

		/// <summary>
		/// Note-off
		/// Code Index Number : 0x8
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="note">0-127</param>
		/// <param name="velocity">0-127</param>
		public void SendMidiNoteOff(int cable, int channel, int note, int velocity)
		{
			SendMidiMessage(unchecked((int)(0x8)), cable, unchecked((int)(0x80)) | (channel &
				 unchecked((int)(0xf))), note, velocity);
		}

		/// <summary>
		/// Note-on
		/// Code Index Number : 0x9
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="note">0-127</param>
		/// <param name="velocity">0-127</param>
		public void SendMidiNoteOn(int cable, int channel, int note, int velocity)
		{
			SendMidiMessage(unchecked((int)(0x9)), cable, unchecked((int)(0x90)) | (channel &
				 unchecked((int)(0xf))), note, velocity);
		}

		/// <summary>
		/// Poly-KeyPress
		/// Code Index Number : 0xa
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="note">0-127</param>
		/// <param name="pressure">0-127</param>
		public void SendMidiPolyphonicAftertouch(int cable, int channel, int note, int pressure
			)
		{
			SendMidiMessage(unchecked((int)(0xa)), cable, unchecked((int)(0xa0)) | (channel &
				 unchecked((int)(0xf))), note, pressure);
		}

		/// <summary>
		/// Control Change
		/// Code Index Number : 0xb
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="function">0-127</param>
		/// <param name="value">0-127</param>
		public void SendMidiControlChange(int cable, int channel, int function, int value
			)
		{
			SendMidiMessage(unchecked((int)(0xb)), cable, unchecked((int)(0xb0)) | (channel &
				 unchecked((int)(0xf))), function, value);
		}

		/// <summary>
		/// Program Change
		/// Code Index Number : 0xc
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="program">0-127</param>
		public void SendMidiProgramChange(int cable, int channel, int program)
		{
			SendMidiMessage(unchecked((int)(0xc)), cable, unchecked((int)(0xc0)) | (channel &
				 unchecked((int)(0xf))), program, 0);
		}

		/// <summary>
		/// Channel Pressure
		/// Code Index Number : 0xd
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="pressure">0-127</param>
		public void SendMidiChannelAftertouch(int cable, int channel, int pressure)
		{
			SendMidiMessage(unchecked((int)(0xd)), cable, unchecked((int)(0xd0)) | (channel &
				 unchecked((int)(0xf))), pressure, 0);
		}

		/// <summary>
		/// PitchBend Change
		/// Code Index Number : 0xe
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="amount">0(low)-8192(center)-16383(high)</param>
		public void SendMidiPitchWheel(int cable, int channel, int amount)
		{
			SendMidiMessage(unchecked((int)(0xe)), cable, unchecked((int)(0xe0)) | (channel &
				 unchecked((int)(0xf))), amount & unchecked((int)(0x7f)), (amount >> 7) & unchecked(
				(int)(0x7f)));
		}

		/// <summary>
		/// Single Byte
		/// Code Index Number : 0xf
		/// </summary>
		/// <param name="cable">0-15</param>
		/// <param name="byte1"></param>
		public void SendMidiSingleByte(int cable, int byte1)
		{
			SendMidiMessage(unchecked((int)(0xf)), cable, byte1, 0, 0);
		}

		/// <summary>RPN message</summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="function">14bits</param>
		/// <param name="value">7bits or 14bits</param>
		public void SendRPNMessage(int cable, int channel, int function, int value)
		{
			SendRPNMessage(cable, channel, (function >> 7) & unchecked((int)(0x7f)), function
				 & unchecked((int)(0x7f)), value);
		}

		/// <summary>RPN message</summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="functionMSB">higher 7bits</param>
		/// <param name="functionLSB">lower 7bits</param>
		/// <param name="value">7bits or 14bits</param>
		public void SendRPNMessage(int cable, int channel, int functionMSB, int functionLSB
			, int value)
		{
			// send the function
			SendMidiControlChange(cable, channel, 101, functionMSB & unchecked((int)(0x7f)));
			SendMidiControlChange(cable, channel, 100, functionLSB & unchecked((int)(0x7f)));
			// send the value
			if ((value >> 7) > 0)
			{
				SendMidiControlChange(cable, channel, 6, (value >> 7) & unchecked((int)(0x7f)));
				SendMidiControlChange(cable, channel, 38, value & unchecked((int)(0x7f)));
			}
			else
			{
				SendMidiControlChange(cable, channel, 6, value & unchecked((int)(0x7f)));
			}
			// send the NULL function
			SendMidiControlChange(cable, channel, 101, unchecked((int)(0x7f)));
			SendMidiControlChange(cable, channel, 100, unchecked((int)(0x7f)));
		}

		/// <summary>NRPN message</summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="function">14bits</param>
		/// <param name="value">7bits or 14bits</param>
		public void SendNRPNMessage(int cable, int channel, int function, int value)
		{
			SendNRPNMessage(cable, channel, (function >> 7) & unchecked((int)(0x7f)), function
				 & unchecked((int)(0x7f)), value);
		}

		/// <summary>NRPN message</summary>
		/// <param name="cable">0-15</param>
		/// <param name="channel">0-15</param>
		/// <param name="functionMSB">higher 7bits</param>
		/// <param name="functionLSB">lower 7bits</param>
		/// <param name="value">7bits or 14bits</param>
		public void SendNRPNMessage(int cable, int channel, int functionMSB, int functionLSB
			, int value)
		{
			// send the function
			SendMidiControlChange(cable, channel, 99, functionMSB & unchecked((int)(0x7f)));
			SendMidiControlChange(cable, channel, 98, functionLSB & unchecked((int)(0x7f)));
			// send the value
			if ((value >> 7) > 0)
			{
				SendMidiControlChange(cable, channel, 6, (value >> 7) & unchecked((int)(0x7f)));
				SendMidiControlChange(cable, channel, 38, value & unchecked((int)(0x7f)));
			}
			else
			{
				SendMidiControlChange(cable, channel, 6, value & unchecked((int)(0x7f)));
			}
			// send the NULL function
			SendMidiControlChange(cable, channel, 101, unchecked((int)(0x7f)));
			SendMidiControlChange(cable, channel, 100, unchecked((int)(0x7f)));
		}
	}
}
