package com.stusherwin.setupmanager.core;

import com.stusherwin.setupmanager.midi.MidiDevice;

public class SyncSetupSender implements SetupSender {
    @Override
    public void sendSetup(MidiDevice midiDevice, Setup setup) {
        if(setup == null)
            return;

        for (SysExMessage msg : setup.getSysExMessages())
        {
            midiDevice.sendSysExMessage(msg);
        }
    }
}