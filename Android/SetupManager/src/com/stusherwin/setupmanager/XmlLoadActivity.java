package com.stusherwin.setupmanager;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.util.List;

import org.xmlpull.v1.XmlPullParserException;

import com.stusherwin.setupmanager.core.MyObject;
import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SysExMessage;
import com.stusherwin.setupmanager.core.XmlSetListLoader;

import android.os.Bundle;
import android.app.Activity;
import android.content.res.AssetManager;
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
			XmlSetListLoader loader = new XmlSetListLoader();
			List<Setup> results = loader.load(stream).getSetups();
			for(Setup setup : results) {
				write(setup.getName() + "\n");
				for(SysExMessage msg : setup.getSysExMessages()) {
					write("\t[" + msg.getBytesString() + "]\n");
				}
			}
		} catch (FileNotFoundException e1) {
			write(e1.getMessage());
		} catch (XmlPullParserException e) {
			write(e.getMessage());
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
