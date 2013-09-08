package com.stusherwin.setupmanager.test.core;

import com.stusherwin.setupmanager.core.*;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.JUnit4;

import java.util.Arrays;
import java.util.List;

@RunWith(JUnit4.class)
public class PerformanceManagerTests {

    private List<Setup> setups;
    private List<Setup> solos;
    private SetList setList;
    private FakeMidiDevice midiDevice;
    private Notifier notifier;
    private SetListStore setListStore;
    private FakeSetupChangeListener setupChangeListener;
    private PerformanceManager performanceManager;

    @Before
    public void setup()
    {
        setups = Arrays.asList( new Setup[] {
            new Setup("Setup1", "Setup1", Arrays.asList(new SysExMessage[] { new SysExMessage(new byte[] { 100, 1, 1 }), new SysExMessage(new byte[] { 100, 1, 2 }) })),
            new Setup("Setup2", "Setup2", Arrays.asList(new SysExMessage[] { new SysExMessage(new byte[] { 100, 2, 1 }), new SysExMessage(new byte[] { 100, 2, 2 }) }))
        });

        solos = Arrays.asList( new Setup[] {
            new Setup("Solo1", "Solo1", Arrays.asList(new SysExMessage[] { new SysExMessage(new byte[] { 101, 1, 1 }), new SysExMessage(new byte[] { 101, 1, 2 }) })),
            new Setup("Solo2", "Solo2", Arrays.asList(new SysExMessage[] { new SysExMessage(new byte[] { 101, 2, 1 }), new SysExMessage(new byte[] { 101, 2, 2 }) }))
        });

        setList = new SetList(setups, solos);

        midiDevice = new FakeMidiDevice();

        notifier = new Notifier() {
            @Override
            public void Notify(String message) {                                                                                                                                 //To change body of implemented methods use File | Settings | File Templates.
            }
        };

        setListStore = new SetListStore() {
            @Override
            public SetList retrieve() {
                return setList;
            }

            @Override
            public void store(SetList setList) {
                PerformanceManagerTests.this.setList = setList;
            }
        };

        setupChangeListener = new FakeSetupChangeListener();

        performanceManager = new PerformanceManager(midiDevice, notifier, setListStore, setupChangeListener, new SyncSetupSender(), new SyncSetupReceiver());
    }

    @Test
    public void loadSetList_loadsSetListFromStore() {
        performanceManager.loadSetList();
        Assert.assertEquals(setups, performanceManager.getSetups());
    }

    @Test
    public void initialize_SetsCurrentSetupToFirstSetupInSetList() {
        setupChangeListener.startListening();
        performanceManager.loadSetList();
        performanceManager.initialize();

        Assert.assertEquals(setups.get(0), performanceManager.getCurrentSetup());
        Assert.assertEquals(setups.get(0), setupChangeListener.latestSelectedSetup);
        Assert.assertEquals(null, setupChangeListener.latestSelectedSolo);
        midiDevice.assertLastSysExesReceived(setups.get(0).getSysExMessages());
    }

    @Test
    public void selectSetupAtIndex_SetsCurrentSetupToSelectedSetup() {
        performanceManager.loadSetList();
        performanceManager.initialize();

        setupChangeListener.startListening();
        performanceManager.selectSetupAtIndex(1);

        Assert.assertEquals(setups.get(1), performanceManager.getCurrentSetup());
        Assert.assertEquals(setups.get(1), setupChangeListener.latestSelectedSetup);
        Assert.assertEquals(null, setupChangeListener.latestSelectedSolo);
        midiDevice.assertLastSysExesReceived(setups.get(1).getSysExMessages());
    }

    @Test
    public void selectSetupAtIndex_WhenInSoloMode_SetsCurrentSetupToSelectedSetup() {
        performanceManager.loadSetList();
        performanceManager.initialize();

        setupChangeListener.startListening();
        performanceManager.selectSetupAtIndex(1);

        Assert.assertEquals(setups.get(1), performanceManager.getCurrentSetup());
        Assert.assertEquals(setups.get(1), setupChangeListener.latestSelectedSetup);
        Assert.assertEquals(null, setupChangeListener.latestSelectedSolo);
        midiDevice.assertLastSysExesReceived(setups.get(1).getSysExMessages());
    }

    @Test
    public void toggleSolo_WhenInSetupMode_SetsCurrentSetupToToggledSolo() {
        performanceManager.loadSetList();
        performanceManager.initialize();

        setupChangeListener.startListening();
        performanceManager.toggleSolo(solos.get(0));

        Assert.assertEquals(solos.get(0), performanceManager.getCurrentSetup());
        Assert.assertFalse(setupChangeListener.selectedSetupWasChanged());
        Assert.assertEquals(solos.get(0), setupChangeListener.latestSelectedSolo);
        midiDevice.assertLastSysExesReceived(solos.get(0).getSysExMessages());
    }

    @Test
    public void toggleSolo_WhenInSoloMode_AndToggledSoloSelected_SetsCurrentSetupBackToPreviousSetup() {
        performanceManager.loadSetList();
        performanceManager.initialize();
        performanceManager.selectSetup(setups.get(1));
        performanceManager.toggleSolo(solos.get(0));

        setupChangeListener.startListening();
        performanceManager.toggleSolo(solos.get(0));

        Assert.assertEquals(setups.get(1), performanceManager.getCurrentSetup());
        Assert.assertFalse(setupChangeListener.selectedSetupWasChanged());
        Assert.assertEquals(null, setupChangeListener.latestSelectedSolo);
        midiDevice.assertLastSysExesReceived(setups.get(1).getSysExMessages());
    }

    @Test
    public void toggleSolo_WhenInSoloMode_AndDifferentSoloSelected_SetsCurrentSetupToToggledSolo() {
        performanceManager.loadSetList();
        performanceManager.initialize();
        performanceManager.selectSetup(setups.get(1));
        performanceManager.toggleSolo(solos.get(0));

        setupChangeListener.startListening();
        performanceManager.toggleSolo(solos.get(1));

        Assert.assertEquals(solos.get(1), performanceManager.getCurrentSetup());
        Assert.assertFalse(setupChangeListener.selectedSetupWasChanged());
        Assert.assertEquals(solos.get(1), setupChangeListener.latestSelectedSolo);
        midiDevice.assertLastSysExesReceived(solos.get(1).getSysExMessages());
    }

    @Test
    public void footSwitchPressed_WhenInSetupMode_SetsCurrentSetupToNextSetupInSetList() {
        performanceManager.loadSetList();
        performanceManager.initialize();
        setupChangeListener.startListening();
        performanceManager.footSwitchPressed();

        Assert.assertEquals(setups.get(1), performanceManager.getCurrentSetup());
        Assert.assertEquals(setups.get(1), setupChangeListener.latestSelectedSetup);
        Assert.assertFalse(setupChangeListener.selectedSoloWasChanged());
        midiDevice.assertLastSysExesReceived(setups.get(1).getSysExMessages());
    }

    @Test
    public void footSwitchPressed_WhenInSoloMode_SetsCurrentSetupBackToPreviousSetup() {
        performanceManager.loadSetList();
        performanceManager.initialize();
        performanceManager.toggleSolo(solos.get(1));

        setupChangeListener.startListening();
        performanceManager.footSwitchPressed();

        Assert.assertEquals(setups.get(0), performanceManager.getCurrentSetup());
        Assert.assertFalse(setupChangeListener.selectedSetupWasChanged());
        Assert.assertEquals(null, setupChangeListener.latestSelectedSolo);
        midiDevice.assertLastSysExesReceived(setups.get(0).getSysExMessages());
    }

    @Test
    public void syncCurrentSetup_OverwritesCurrentSetupWithSetupFromMidiDevice() {
        performanceManager.loadSetList();
        performanceManager.initialize();
        performanceManager.selectSetupAtIndex(1);

        setupChangeListener.startListening();
        performanceManager.syncCurrentSetup();

        Assert.assertEquals(midiDevice.currentSetupSysExMessages, setups.get(1).getSysExMessages());
        Assert.assertFalse(setupChangeListener.selectedSetupWasChanged());
        Assert.assertFalse(setupChangeListener.selectedSoloWasChanged());
    }
}