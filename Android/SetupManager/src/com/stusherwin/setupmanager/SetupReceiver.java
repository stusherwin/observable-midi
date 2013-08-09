package com.stusherwin.setupmanager;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import jp.kshoji.driver.midi.device.MidiInputDevice;

import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SysExMessage;
import com.stusherwin.setupmanager.midi.MidiManager;

public class SetupReceiver {
	private MidiManager _midiManager;
	private byte[] REQUEST_SETUP = new byte[]
    {
        (byte)0xF0, 0x41, 0x10, 0x00, 0x00, 0x2B, 0x11, 0x10, 0x00, 0x00, 0x00, 0x00, 0x07, 0x0F, 0x0B, 0x41, (byte)0xF7
    };
	private int END_OF_MESSAGES_TIMEOUT = 1000;

	public SetupReceiver(MidiManager midiManager) {
		_midiManager = midiManager;
	}

	public Setup receive() throws InterruptedException {
		final List<SysExMessage> sysexes = new ArrayList<SysExMessage>();
        final Date lastMessageReceived = new Date(2030, 1, 1);
        
		// receive setup
		_midiManager.addInputListener(new MidiInputEventListener() {
			@Override
			public void onMidiSystemExclusive(MidiInputDevice sender, int cable,
					byte[] systemExclusive) {
				sysexes.add(new SysExMessage(systemExclusive));
				lastMessageReceived.setTime(new Date().getTime());
			}
		});
		
		// request setup
		_midiManager.sendMidiSystemExclusive(0, REQUEST_SETUP);
		
		while (new Date().getTime() - lastMessageReceived.getTime() < END_OF_MESSAGES_TIMEOUT)
        {
            Thread.sleep(10);
        }
		
		return Setup.load(sysexes);
	}
}
