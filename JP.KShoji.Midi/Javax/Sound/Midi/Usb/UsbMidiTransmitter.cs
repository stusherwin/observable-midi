/*
 * This code is derived from MyJavaLibrary (http://somelinktomycoollibrary)
 * 
 * If this is an open source Java library, include the proper license and copyright attributions here!
 */

using Android.Hardware.Usb;
using Android.Util;
using JP.KShoji.Driver.Midi.Device;
using JP.KShoji.Driver.Midi.Listener;
using JP.KShoji.Driver.Midi.Util;
using JP.KShoji.Javax.Sound.Midi;
using JP.KShoji.Javax.Sound.Midi.Usb;
using Sharpen;

namespace JP.KShoji.Javax.Sound.Midi.Usb
{
	/// <summary>
	/// <see cref="JP.KShoji.Javax.Sound.Midi.Transmitter">JP.KShoji.Javax.Sound.Midi.Transmitter
	/// 	</see>
	/// implementation
	/// </summary>
	/// <author>K.Shoji</author>
	public sealed class UsbMidiTransmitter : Transmitter
	{
		private readonly UsbDevice usbDevice;

		private readonly UsbDeviceConnection usbDeviceConnection;

		private readonly UsbInterface usbInterface;

		private readonly UsbEndpoint inputEndpoint;

		private MidiInputDevice inputDevice;

		internal Receiver receiver;

		public UsbMidiTransmitter(UsbDevice usbDevice, UsbDeviceConnection usbDeviceConnection
			, UsbInterface usbInterface, UsbEndpoint inputEndpoint)
		{
			this.usbDevice = usbDevice;
			this.usbDeviceConnection = usbDeviceConnection;
			this.usbInterface = usbInterface;
			this.inputEndpoint = inputEndpoint;
		}

		public void SetReceiver(Receiver receiver)
		{
			this.receiver = receiver;
		}

		public Receiver GetReceiver()
		{
			return receiver;
		}

		public void Open()
		{
			inputDevice = new MidiInputDevice(usbDevice, usbDeviceConnection, usbInterface, inputEndpoint
				, new UsbMidiTransmitter.OnMidiInputEventListenerImpl(this));
		}

		public void Close()
		{
			if (inputDevice != null)
			{
				inputDevice.Stop();
			}
		}

		internal class OnMidiInputEventListenerImpl : OnMidiInputEventListener
		{
			public virtual void OnMidiMiscellaneousFunctionCodes(MidiInputDevice sender, int 
				cable, int byte1, int byte2, int byte3)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						SysexMessage message = new SysexMessage();
						message.SetMessage(new byte[] { unchecked((byte)(byte1 & unchecked((int)(0xff))))
							, unchecked((byte)(byte2 & unchecked((int)(0xff)))), unchecked((byte)(byte3 & unchecked(
							(int)(0xff)))) }, 3);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiCableEvents(MidiInputDevice sender, int cable, int byte1
				, int byte2, int byte3)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						SysexMessage message = new SysexMessage();
						message.SetMessage(new byte[] { unchecked((byte)(byte1 & unchecked((int)(0xff))))
							, unchecked((byte)(byte2 & unchecked((int)(0xff)))), unchecked((byte)(byte3 & unchecked(
							(int)(0xff)))) }, 3);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiSystemCommonMessage(MidiInputDevice sender, int cable, 
				byte[] bytes)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						SysexMessage message = new SysexMessage();
						message.SetMessage(bytes, bytes.Length);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiSystemExclusive(MidiInputDevice sender, int cable, byte
				[] systemExclusive)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						SysexMessage message = new SysexMessage();
						message.SetMessage(systemExclusive, systemExclusive.Length);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiNoteOff(MidiInputDevice sender, int cable, int channel, 
				int note, int velocity)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						ShortMessage message = new ShortMessage();
						message.SetMessage(ShortMessage.NOTE_OFF, channel, note, velocity);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiNoteOn(MidiInputDevice sender, int cable, int channel, 
				int note, int velocity)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						ShortMessage message = new ShortMessage();
						message.SetMessage(ShortMessage.NOTE_ON, channel, note, velocity);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiPolyphonicAftertouch(MidiInputDevice sender, int cable, 
				int channel, int note, int pressure)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						ShortMessage message = new ShortMessage();
						message.SetMessage(ShortMessage.POLY_PRESSURE, channel, note, pressure);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiControlChange(MidiInputDevice sender, int cable, int channel
				, int function, int value)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						ShortMessage message = new ShortMessage();
						message.SetMessage(ShortMessage.CONTROL_CHANGE, channel, function, value);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiProgramChange(MidiInputDevice sender, int cable, int channel
				, int program)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						ShortMessage message = new ShortMessage();
						message.SetMessage(ShortMessage.PROGRAM_CHANGE, channel, program, 0);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiChannelAftertouch(MidiInputDevice sender, int cable, int
				 channel, int pressure)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						ShortMessage message = new ShortMessage();
						message.SetMessage(ShortMessage.CHANNEL_PRESSURE, channel, pressure, 0);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiPitchWheel(MidiInputDevice sender, int cable, int channel
				, int amount)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						ShortMessage message = new ShortMessage();
						message.SetMessage(ShortMessage.PITCH_BEND, channel, (amount >> 7) & unchecked((int
							)(0x7f)), amount & unchecked((int)(0x7f)));
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiSingleByte(MidiInputDevice sender, int cable, int byte1
				)
			{
				if (this._enclosing.receiver != null)
				{
					try
					{
						SysexMessage message = new SysexMessage();
						message.SetMessage(new byte[] { unchecked((byte)(byte1 & unchecked((int)(0xff))))
							 }, 1);
						this._enclosing.receiver.Send(message, -1);
					}
					catch (InvalidMidiDataException e)
					{
						Log.Info(Constants.TAG, "InvalidMidiDataException", e);
					}
				}
			}

			public virtual void OnMidiRPNReceived(MidiInputDevice sender, int cable, int channel
				, int function, int value, int valueLSB)
			{
			}

			// do nothing in this implementation
			public virtual void OnMidiNRPNReceived(MidiInputDevice sender, int cable, int channel
				, int function, int value, int valueLSB)
			{
			}

			internal OnMidiInputEventListenerImpl(UsbMidiTransmitter _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly UsbMidiTransmitter _enclosing;
			// do nothing in this implementation
		}
	}
}
