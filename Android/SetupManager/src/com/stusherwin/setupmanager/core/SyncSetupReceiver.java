package com.stusherwin.setupmanager.core;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import com.stusherwin.setupmanager.midi.MidiDevice;
import com.stusherwin.setupmanager.midi.MidiInputEventListener;
import jp.kshoji.driver.midi.device.MidiInputDevice;

import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SysExMessage;
import com.stusherwin.setupmanager.midi.MidiManager;

public class SyncSetupReceiver implements SetupReceiver {
	private byte[] REQUEST_SETUP = new byte[]
    {
        (byte)0xF0, 0x41, 0x10, 0x00, 0x00, 0x2B, 0x11, 0x10, 0x00, 0x00, 0x00, 0x00, 0x07, 0x0F, 0x0B, 0x41, (byte)0xF7
    };
	private int END_OF_MESSAGES_TIMEOUT = 1000;

	@Override
    public Setup receiveSetup(MidiDevice midiDevice) throws InterruptedException {
		final List<SysExMessage> sysexes = new ArrayList<SysExMessage>();
        final Date lastMessageReceived = new Date(2030, 1, 1);
        
		// receiveSetup setup
        MidiInputEventListener inputListener = new MidiInputEventListener() {
            @Override
            public void onMidiSystemExclusive(MidiInputDevice sender, int cable,
                                              byte[] systemExclusive) {
                sysexes.add(new SysExMessage(systemExclusive));
                lastMessageReceived.setTime(new Date().getTime());
            }
        };

        midiDevice.addInputListener(inputListener);
		
		// request setup
		midiDevice.sendMidiSystemExclusive(0, REQUEST_SETUP);
		
		while (new Date().getTime() - lastMessageReceived.getTime() < END_OF_MESSAGES_TIMEOUT)
        {
            Thread.sleep(10);
        }

        midiDevice.removeInputListener(inputListener);
		
		return Setup.load(sysexes);
	}
}
