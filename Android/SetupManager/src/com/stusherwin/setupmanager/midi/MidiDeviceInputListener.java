package com.stusherwin.setupmanager.midi;

import com.stusherwin.setupmanager.core.SysExMessage;

public interface MidiDeviceInputListener {
    void onSysExMessage(SysExMessage sysExMessage);
}
