package com.stusherwin.setupmanager.android;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.List;

import com.stusherwin.setupmanager.R;
import com.stusherwin.setupmanager.core.XmlSetListStore;
import org.xmlpull.v1.XmlPullParserException;

import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SysExMessage;

import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;
import android.widget.TextView;

public class XmlLoadActivity extends Activity {
	private TextView _textView;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_xml);
		
		_textView = (TextView) findViewById(R.id.textView1);
		_textView.setText("Hi there\n\n");
		
		try {
			InputStream stream = this.getAssets().open("setups.xml");
			XmlSetListStore loader = new XmlSetListStore(new ActivitySetListFileStreamProvider(this));
			List<Setup> results = loader.retrieve().getSetups();
			for(Setup setup : results) {
				write(setup.getName() + "\n");
				for(SysExMessage msg : setup.getSysExMessages()) {
					write("\t[" + msg.getBytesString() + "]\n");
				}
			}
		} catch (FileNotFoundException e1) {
			write(e1.getMessage());
		} catch (IOException e) {
			write(e.getMessage());
		}
	}
	
	private void write(String text) {
		_textView.setText(_textView.getText() + text);
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
	}
}
