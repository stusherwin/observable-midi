package com.stusherwin.setupmanager.android;

import java.util.ArrayList;
import java.util.List;

import com.stusherwin.setupmanager.R;
import com.stusherwin.setupmanager.core.*;

import com.stusherwin.setupmanager.midi.AllAttachedUsbMidiDevices;

import android.os.Bundle;
import android.app.Activity;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.view.WindowManager;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;

public class SetListActivity extends Activity implements SetupChangeListener, Notifier {
    private PerformanceManager _performanceManager;
    private SelectedItemAdapter<Setup> _setListAdapter;
    private ListView _listView;
	private List<SoloButton> _soloButtons = new ArrayList<SoloButton>();
	private Button _sync;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                                WindowManager.LayoutParams.FLAG_FULLSCREEN);

		setContentView(R.layout.activity_set_list);

        initializePerformanceManager();
        initializeListView();
        initializeSoloButtons();
        initializeSyncButton();
	}

    @Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
        _performanceManager.footSwitchPressed();
		
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

	    _performanceManager.dispose();
	}

    @Override
    public void selectedSetupChanged(Setup selectedSetup) {
        _setListAdapter.selectItem(selectedSetup);
        _listView.smoothScrollToPositionFromTop(_setListAdapter.getSelectedItemPosition(), 580 );
    }

    @Override
    public void selectedSoloChanged(Setup selectedSolo) {
        for(SoloButton sb : _soloButtons) {
            sb.button.setBackgroundResource(sb.solo == selectedSolo ? R.drawable.selected_button_shape : R.drawable.button_shape);
        }
    }

    @Override
    public void Notify(String message) {
        Toast.makeText(SetListActivity.this, message, Toast.LENGTH_SHORT).show();
    }

    private void initializePerformanceManager() {
        AllAttachedUsbMidiDevices midiDevice = new AllAttachedUsbMidiDevices();
        midiDevice.initialize(getApplicationContext());

        _performanceManager = new PerformanceManager(
            midiDevice,
            this,
            new XmlSetListStore(new ActivitySetListFileStreamProvider(this)),
            this,
            new AsyncSetupSender(),
            new SyncSetupReceiver());

        _performanceManager.loadSetList();
    }

    private void initializeListView() {
        _setListAdapter = new SelectedItemAdapter<Setup>(this, _performanceManager.getSetups());

        _listView = (ListView)findViewById( android.R.id.list );
        _listView.setAdapter( _setListAdapter );
        _listView.setEmptyView(findViewById( android.R.id.empty ));
        _listView.setOnItemClickListener(new OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                _performanceManager.selectSetupAtIndex(position);
            }
        });
    }

    private void initializeSoloButtons() {
        OnClickListener soloButtonOnClickListener = new OnClickListener() {
            @Override
            public void onClick(View view) {
                Setup solo = findSoloForButton((Button)view);
                _performanceManager.toggleSolo(solo);
            }
        };

        int[] soloButtonIds = { R.id.solo1, R.id.solo2, R.id.solo3 };

        for(int i = 0; i < soloButtonIds.length; i++) {
            Button button = (Button) findViewById(soloButtonIds[i]);
            Setup solo = _performanceManager.getSoloAtIndex(i);

            button.setText(solo.getName());
            button.setOnClickListener(soloButtonOnClickListener);
            _soloButtons.add(new SoloButton(button, solo));
        }
    }

    private void initializeSyncButton() {
        _sync = (Button)findViewById( R.id.solo4 );
        _sync.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View v) {
                _performanceManager.syncCurrentSetup();
            }
        });
    }

    private Setup findSoloForButton(Button button) {
        for(SoloButton sb : _soloButtons) {
            if(sb.button == button)
                return sb.solo;
        }

        return null;
    }

    private class SoloButton {
        public Button button;
        public Setup solo;

        public SoloButton(Button button, Setup solo) {
            this.button = button;
            this.solo = solo;
        }
    }
}