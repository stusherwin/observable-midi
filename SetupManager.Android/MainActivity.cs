using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware.Usb;
using System.Linq;
using JP.KShoji.Javax.Sound.Midi;
using JP.KShoji.Driver.Midi.Activity;

namespace SetupManager.Android
{
	[Activity (Label = "SetupManager.Android", MainLauncher = true)]
	public class MainActivity : AbstractSingleMidiActivity
	{
        private TextView _textView;
        private MidiDevice _device;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
            _textView = FindViewById<TextView> (Resource.Id.textView1);
			
            _textView.Text = "Hi there. USB devices: \n";

            UsbManager manager = (UsbManager)GetSystemService(Context.UsbService);

            _textView.Text += String.Join("\n", manager.DeviceList.Values.Select(v => 
                                                                                 String.Format("DeviceId: {0}, Name: {1}, ProductID: {2}, VendorId: {3}, Class: {4}, Subclass: {5}",
                          //RD700GX: 2002,        /dev/bus/usb/002/002  122           1410 
                          v.DeviceId,  v.DeviceName,         v.ProductId,  v.VendorId, v.DeviceClass, v.DeviceSubclass)));

            _textView.Text += "\nInitializing midi...\n";

            MidiSystem.Initialize(this);
		}

        #region implemented abstract members of AbstractSingleMidiActivity

        public override void OnDeviceDetached(UsbDevice arg1)
        {
        }

        public override void OnDeviceAttached(UsbDevice arg1)
        {

            try {
                _textView.Text += "\nMidi Devices:\n";
                var info = MidiSystem.GetMidiDeviceInfo();
                foreach(var x in info) {
                    _textView.Text += "\n" + x.GetName();
                }
                _device = MidiSystem.GetMidiDevice(info[0]);

                    _device.Open();
                    _device.GetTransmitter().SetReceiver(new MidiReceiver(_textView));

            }
            catch(Exception ex) {
                _textView.Text += "\nError: " + ex.Message;
            }
        }

        public override void OnMidiCableEvents(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4, int arg5)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiChannelAftertouch(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiControlChange(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4, int arg5)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiMiscellaneousFunctionCodes(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4, int arg5)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiNoteOff(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4, int arg5)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiNoteOn(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4, int arg5)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiPitchWheel(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiPolyphonicAftertouch(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4, int arg5)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiProgramChange(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3, int arg4)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiSingleByte(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, int arg3)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiSystemCommonMessage(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, byte[] arg3)
        {
            throw new NotImplementedException();
        }


        public override void OnMidiSystemExclusive(JP.KShoji.Driver.Midi.Device.MidiInputDevice arg1, int arg2, byte[] arg3)
        {
            throw new NotImplementedException();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(_device != null && _device.IsOpen())
            {
                _device.Close();
            }
            MidiSystem.Terminate();
        }


        #endregion

        private class MidiReceiver : Receiver {
            TextView _textView;
            public MidiReceiver(TextView textView) {
                _textView = textView;
            }

            public void Send(MidiMessage message, long timestamp) {
                _textView.Text += "\nReceived: " + string.Join(" ", message.GetMessage().Select(b => b.ToString("X2")));
            }

            public void Close() {
                _textView.Text += "\nClosed.";
            }
        }
	}
}


