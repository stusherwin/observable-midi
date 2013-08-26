package com.stusherwin.setupmanager.midi;

import com.stusherwin.setupmanager.core.Disposable;
import jp.kshoji.driver.midi.listener.OnMidiInputEventListener;

public interface MidiDevice extends Disposable {
    void sendMidiSystemExclusive(int channel, byte[] bytes);

    void addInputListener(OnMidiInputEventListener inputListener);
    void removeInputListener(OnMidiInputEventListener inputListener);
}
