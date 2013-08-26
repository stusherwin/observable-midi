package com.stusherwin.setupmanager.core;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.stusherwin.setupmanager.midi.MidiDevice;
import com.stusherwin.setupmanager.midi.MidiDeviceInputListener;
import jp.kshoji.driver.midi.device.MidiInputDevice;

public class SyncSetupReceiver implements SetupReceiver {
	private int END_OF_MESSAGES_TIMEOUT = 1000;

	@Override
    public Setup receiveSetup(MidiDevice midiDevice) {
		final List<SysExMessage> sysexes = new ArrayList<SysExMessage>();
        final Date lastMessageReceived = new Date(2030, 1, 1);
        
		// receiveSetup setup
        MidiDeviceInputListener inputListener = new MidiDeviceInputListener() {
            @Override
            public void onSysExMessage(SysExMessage sysExMessage) {
                sysexes.add(sysExMessage);
                lastMessageReceived.setTime(new Date().getTime());
            }
        };

        midiDevice.addInputListener(inputListener);
		
		// request setup
		midiDevice.sendSysExMessage(midiDevice.getRequestSetupMessage());
		
		while (new Date().getTime() - lastMessageReceived.getTime() < END_OF_MESSAGES_TIMEOUT)
        {
            try {
                Thread.sleep(10);
            } catch (InterruptedException e) {
                e.printStackTrace();  //To change body of catch statement use File | Settings | File Templates.
            }
        }

        midiDevice.removeInputListener(inputListener);
		
		return Setup.load(sysexes);
	}
}