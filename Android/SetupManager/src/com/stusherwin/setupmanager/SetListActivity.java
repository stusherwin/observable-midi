package com.stusherwin.setupmanager;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;

import org.xmlpull.v1.XmlPullParserException;

import jp.kshoji.driver.midi.device.MidiInputDevice;

import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SysExMessage;
import com.stusherwin.setupmanager.core.XmlSetupLoader;
import com.stusherwin.setupmanager.midi.MidiManager;

import android.os.Bundle;
import android.app.Activity;
import android.app.ListActivity;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.Toast;

public class SetListActivity extends Activity {
	private MidiManager _midiManager;
	private ListView _listView;
	private SetListAdapter _setListAdapter;
	private SetupSender _setupSender;
	
	private void selectPosition(int position) {
		_setListAdapter.selectSetupAtPosition(position);
		Setup setup = _setListAdapter.getSelectedSetup();
		
		Toast.makeText(SetListActivity.this, "Sending " + setup.getName() + "...", Toast.LENGTH_SHORT).show();
		_listView.smoothScrollToPositionFromTop(_setListAdapter.getSelectedSetupPosition(), 580 );
		sendSetup(setup);
	}

	private void sendSetup(Setup setup) {
		if(_setupSender != null && _setupSender.isAlive() && !_setupSender.isCancelled()) {
			_setupSender.cancel();
		}
		_setupSender = new SetupSender(_midiManager, setup);
		_setupSender.start();
	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, 
                                WindowManager.LayoutParams.FLAG_FULLSCREEN);
        
		setContentView(R.layout.activity_set_list);
		
		try {
			InputStream stream = this.getAssets().open("setups.xml");
			XmlSetupLoader loader = new XmlSetupLoader();
			List<Setup> setups = loader.load(stream);
			
			_setListAdapter = new SetListAdapter( this, setups);
			
			_listView = (ListView)findViewById( android.R.id.list );
			_listView.setAdapter( _setListAdapter );
			//_listView.setAdapter( new CustomAdapter( this, setups ) );
			_listView.setEmptyView(findViewById( android.R.id.empty ));
			_listView.setOnItemClickListener(new OnItemClickListener() {
				@Override
				public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
					selectPosition(position);
				}
			});
			
			_midiManager = new MidiManager();
			_midiManager.initialize(getApplicationContext());
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (XmlPullParserException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		selectPosition(_setListAdapter.getSelectedSetupPosition() + 1);
		
		return super.onKeyDown(keyCode, event);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.set_list, menu);
		return true;
	}
	
	@Override
	public void onDestroy() {
		super.onDestroy();

	    _midiManager.destroy();
	}
}
