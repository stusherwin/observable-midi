package com.stusherwin.setupmanager.core;

import com.stusherwin.setupmanager.midi.MidiDevice;

public interface SetupReceiver {
    Setup receiveSetup(MidiDevice midiDevice);
}
