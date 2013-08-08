package com.stusherwin.setupmanager;

import jp.kshoji.driver.midi.device.MidiInputDevice;

import com.stusherwin.setupmanager.midi.MidiManager;

import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class MidiActivity extends Activity {
	private TextView _textView;
	private MidiManager _midiManager;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_midi);
		
		_textView = (TextView) findViewById(R.id.textView1);
		_textView.setText("Hi there");
		
		_midiManager = new MidiManager();
		_midiManager.initialize(getApplicationContext());
		_midiManager.addInputListener(new MidiInputEventListener() {
			@Override
			public void onMidiNoteOn(MidiInputDevice sender, int cable,
					int channel, int note, int velocity) {
				_textView.setText(_textView.getText() + "\n Note received! " + note);				
			} 
		});
		
		Button noteOn = (Button) findViewById(R.id.noteOn);	
		Button noteOff = (Button) findViewById(R.id.noteOff);
		
		noteOn.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View arg0) {
				_midiManager.sendMidiNoteOn(0, 0, 45, 100);
				_midiManager.sendMidiNoteOn(0, 0, 55, 100);
				_midiManager.sendMidiNoteOn(0, 0, 81, 100);
			}
		});
		
		noteOff.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View arg0) {
				_midiManager.sendMidiNoteOff(0, 0, 45, 100);
				_midiManager.sendMidiNoteOff(0, 0, 55, 100);
				_midiManager.sendMidiNoteOff(0, 0, 81, 100);
			}
		});
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.midi, menu);
		return true;
	}
	
	@Override
	public void onDestroy() {
		super.onDestroy();

	    _midiManager.destroy();
	}
}
