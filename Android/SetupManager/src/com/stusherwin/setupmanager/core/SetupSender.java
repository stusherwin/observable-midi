package com.stusherwin.setupmanager.core;

import com.stusherwin.setupmanager.midi.MidiDevice;

public interface SetupSender {
    void sendSetup(MidiDevice midiDevice, Setup setup);
}