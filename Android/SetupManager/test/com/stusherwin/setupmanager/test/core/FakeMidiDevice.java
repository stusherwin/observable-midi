package com.stusherwin.setupmanager.test.core;

import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SysExMessage;
import com.stusherwin.setupmanager.midi.MidiDevice;
import com.stusherwin.setupmanager.midi.MidiDeviceInputListener;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class FakeMidiDevice implements MidiDevice {
    static final SysExMessage requestSetup = new SysExMessage(new byte[] { 1, 2, 3 });
    List<MidiDeviceInputListener> inputListeners = new ArrayList<MidiDeviceInputListener>();

    public static final List<SysExMessage> currentSetupSysExMessages = Arrays.asList(new SysExMessage[] {
        new SysExMessage(new byte[] { 4, 5, 6 }),
        new SysExMessage(new byte[] { 7, 8, 9 })
    });
    public List<SysExMessage> receivedSysExMessages = new ArrayList<SysExMessage>();

    @Override
    public SysExMessage getRequestSetupMessage() {
        return requestSetup;
    }

    @Override
    public SysExMessage getSetupChangeMessage() {
        return null;
    }

    @Override
    public void sendSysExMessage(SysExMessage sysExMessage) {
        receivedSysExMessages.add(sysExMessage);

        if(sysExMessage == requestSetup)
        {
            for(SysExMessage msg : currentSetupSysExMessages) {
                broadcast(msg);
            }
        }
    }

    @Override
    public void addInputListener(MidiDeviceInputListener inputListener) {
        inputListeners.add(inputListener);
    }

    @Override
    public void removeInputListener(MidiDeviceInputListener inputListener) {
        inputListeners.remove(inputListener);
    }

    @Override
    public void dispose() {
    }

    private void broadcast(SysExMessage sysExMessage) {
        for(MidiDeviceInputListener listener : inputListeners) {
            listener.onSysExMessage(sysExMessage);
        }
    }
}