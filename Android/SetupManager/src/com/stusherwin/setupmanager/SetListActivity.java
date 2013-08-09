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

import com.stusherwin.setupmanager.core.SetList;
import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SysExMessage;
import com.stusherwin.setupmanager.core.XmlSetListLoader;
import com.stusherwin.setupmanager.midi.MidiManager;

import android.os.Bundle;
import android.app.Activity;
import android.app.ListActivity;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.view.WindowManager;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SimpleAdapter;
import android.widget.Toast;

public class SetListActivity extends Activity {
	private MidiManager _midiManager;
	private ListView _listView;
	private SetListAdapter _setListAdapter;
	private SetupSender _setupSender;
	private SetList _setList;
	
	private List<Button> _soloButtons = new ArrayList<Button>();
	private boolean[] _soloSelected = new boolean[4]; 
	
	private void selectPosition(int position) {
		_setListAdapter.selectSetupAtPosition(position);
		Setup setup = _setListAdapter.getSelectedSetup();
		
		_listView.smoothScrollToPositionFromTop(_setListAdapter.getSelectedSetupPosition(), 580 );
		
		sendSetup(setup);
	}
	
	private void selectSolo(Button button) {
		int selectedSoloIdx = _soloButtons.indexOf(button);
		
		for(int i = 0; i < _soloSelected.length; i++) {
			_soloSelected[i] = !_soloSelected[i] && i == selectedSoloIdx;
			_soloButtons.get(i).setBackgroundResource(_soloSelected[i] ? R.drawable.selected_button_shape : R.drawable.button_shape);
		}
		
		if(_soloSelected[selectedSoloIdx]) {
			sendSetup(_setList.getSolos().get(selectedSoloIdx));
		} else {
			sendSetup(_setListAdapter.getSelectedSetup());
		}
	}

	private void sendSetup(Setup setup) {
		Toast.makeText(SetListActivity.this, "Sending " + setup.getName() + "...", Toast.LENGTH_SHORT).show();
		
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
			XmlSetListLoader loader = new XmlSetListLoader();
			
			_setList = loader.load(stream);
			
			_setListAdapter = new SetListAdapter( this, _setList.getSetups());
			
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
			
			_soloButtons.add((Button)findViewById( R.id.solo1 ));
			_soloButtons.add((Button)findViewById( R.id.solo2 ));
			_soloButtons.add((Button)findViewById( R.id.solo3 ));
			_soloButtons.add((Button)findViewById( R.id.solo4 ));
			
			OnClickListener onClickListener = new OnClickListener() {
				@Override
				public void onClick(View view) {
					selectSolo((Button)view);
				}
			};
			
			for(int i = 0; i < _soloButtons.size(); i++) {
				_soloButtons.get(i).setText(_setList.getSolos().get(i).getName());
				_soloButtons.get(i).setOnClickListener(onClickListener);
			}
			
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
