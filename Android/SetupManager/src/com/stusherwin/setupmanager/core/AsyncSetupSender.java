package com.stusherwin.setupmanager.core;

import com.stusherwin.setupmanager.midi.MidiDevice;

public class AsyncSetupSender implements SetupSender {
    private SetupSenderThread _setupSenderThread;

    @Override
    public void sendSetup(MidiDevice midiDevice, Setup setup) {
        if(setup == null)
            return;

        if(_setupSenderThread != null && _setupSenderThread.isAlive() && !_setupSenderThread.isCancelled()) {
            _setupSenderThread.cancel();
        }
        _setupSenderThread = new SetupSenderThread(midiDevice, setup);
        _setupSenderThread.start();
    }

    private class SetupSenderThread extends Thread {
        private MidiDevice _midiDevice;
        private Setup _setup;
        private boolean _cancelled = false;

        public SetupSenderThread(MidiDevice midiDevice, Setup setup) {
            _midiDevice = midiDevice;
            _setup = setup;
        }

        @Override
        public void run() {
            for (SysExMessage msg : _setup.getSysExMessages())
            {
                if(_cancelled) {
                    break;
                }

                _midiDevice.sendSysExMessage(msg);

                try {
                    Thread.sleep(40);
                } catch (InterruptedException e) {
                    // TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
        }

        public void cancel() {
            _cancelled = true;
        }

        public boolean isCancelled() {
            return _cancelled;
        }
    }
}
