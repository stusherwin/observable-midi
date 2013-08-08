package com.stusherwin.setupmanager;

import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SysExMessage;
import com.stusherwin.setupmanager.midi.MidiManager;

public class SetupSender extends Thread {
	private MidiManager _midiManager;
	private Setup _setup;
	private boolean _cancelled = false;
	
	public SetupSender(MidiManager midiManager, Setup setup) {
		_midiManager = midiManager;
		_setup = setup;
	}
	
	@Override
	public void run() {
		for (SysExMessage msg : _setup.getSysExMessages())
        {
			if(_cancelled) {
				break;
			}
			
			_midiManager.sendMidiSystemExclusive(0, msg.getBytes());
			
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
