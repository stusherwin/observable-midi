package com.stusherwin.setupmanager.midi;

import com.stusherwin.setupmanager.core.Disposable;
import com.stusherwin.setupmanager.core.SysExMessage;

public interface MidiDevice extends Disposable {
    SysExMessage getRequestSetupMessage();
    SysExMessage getSetupChangeMessage();
    void sendSysExMessage(SysExMessage sysExMessage);
    void addInputListener(MidiDeviceInputListener inputListener);
    void removeInputListener(MidiDeviceInputListener inputListener);
}
