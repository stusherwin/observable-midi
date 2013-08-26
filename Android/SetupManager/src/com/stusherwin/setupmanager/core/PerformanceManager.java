package com.stusherwin.setupmanager.core;

import com.stusherwin.setupmanager.midi.MidiDevice;
import java.util.List;

public class PerformanceManager implements Disposable {
    private MidiDevice _midiDevice;
    private SetupSender _setupSender;
    private SetupReceiver _setupReceiver;
    private Notifier _notifier;
    private SetListStore _setListStore;
    private SetupChangeListener _setupChangeListener;

    private SetList _setList;
    private Setup _selectedSetup;
    private Setup _selectedSolo;

    public PerformanceManager(
            MidiDevice midiDevice,
            Notifier notifier,
            SetListStore setListStore,
            SetupChangeListener setupChangeListener,
            SetupSender setupSender,
            SetupReceiver setupReceiver) {
        _midiDevice = midiDevice;
        _notifier = notifier;
        _setListStore = setListStore;
        _setupChangeListener = setupChangeListener;
        _setupSender = setupSender;
        _setupReceiver = setupReceiver;
    }

    public void loadSetList() {
        _setList = _setListStore.retrieve();
    }

    public void selectSetupAtIndex(int setupIndex) {
        selectSetup(_setList.getSetupAtIndex(setupIndex));
    }

    public void selectSetup(Setup setup) {
        _selectedSetup = setup;
        _selectedSolo = null;

        _setupChangeListener.selectedSetupChanged(_selectedSetup);
        _setupChangeListener.selectedSoloChanged(_selectedSolo);
        sendSetup(getCurrentSetup());
    }

    public void toggleSolo(Setup solo) {
        if(_selectedSolo == solo) {
            _selectedSolo = null;
        } else {
            _selectedSolo = solo;
        }

        _setupChangeListener.selectedSoloChanged(_selectedSolo);
        sendSetup(getCurrentSetup());
    }

    public void footSwitchPressed() {
        if(_selectedSolo != null) {
            _selectedSolo = null;
            _setupChangeListener.selectedSoloChanged(_selectedSolo);
        }
        else {
            _selectedSetup = _setList.getNextSetup(_selectedSetup);
            _setupChangeListener.selectedSetupChanged(_selectedSetup);
        }

        sendSetup(getCurrentSetup());
    }

    public void syncCurrentSetup() {
        Setup currentSetup = getCurrentSetup();
        if(currentSetup == null)
            return;

        try {
            Setup setup = _setupReceiver.receiveSetup(_midiDevice);
            currentSetup.setSysExMessages(setup.getSysExMessages());

            _setListStore.store(_setList);
        } catch (InterruptedException e1) {
            // TODO Auto-generated catch block
            e1.printStackTrace();
        }
    }

    public Setup getCurrentSetup() {
        if(_selectedSolo != null) {
            return _selectedSolo;
        } else if(_selectedSetup != null) {
            return _selectedSetup;
        }
        return null;
    }

    @Override
    public void dispose() {
        _midiDevice.dispose();
    }

    private void sendSetup(Setup setup) {
        if(setup == null)
            return;

        _notifier.Notify("Sending " + setup.getName() + "...");
        _setupSender.sendSetup(_midiDevice, setup);
    }

    public Setup getSoloAtIndex(int index) {
        return _setList.getSoloAtIndex(index);
    }

    public List<Setup> getSetups() {
        return _setList.getSetups();
    }
}