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
    private SetupChangeListener setupChangeListener;
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
            public void store(SetList _setList) { }
        };

        setupChangeListener = new SetupChangeListener() {
            @Override
            public void selectedSetupChanged(Setup selectedSetup) {
                //To change body of implemented methods use File | Settings | File Templates.
            }

            @Override
            public void selectedSoloChanged(Setup selectedSolo) {
                //To change body of implemented methods use File | Settings | File Templates.
            }
        };

        performanceManager = new PerformanceManager(midiDevice, notifier, setListStore, setupChangeListener, new SyncSetupSender(), new SyncSetupReceiver());
    }

    @Test
    public void loadSetList_loadsSetListFromStore() {
        performanceManager.loadSetList();
        Assert.assertEquals(setups, performanceManager.getSetups());
    }

    @Test
    public void whenSetListLoaded_SetsCurrentSetupToFirstSetupInSetList() {
        performanceManager.loadSetList();
        Assert.assertEquals(setups.get(0), performanceManager.getCurrentSetup());
        assertLastSysExesReceivedWereFromSetup(setups.get(0));
    }

    private void assertLastSysExesReceivedWereFromSetup(Setup setup) {
        int sysExesLength = setup.getSysExMessages().size();
        List<SysExMessage> receivedSysExes = midiDevice.receivedSysExMessages;
        for(int i = 0; i < sysExesLength; i++) {
            Assert.assertEquals(setup.getSysExMessages().get(i), receivedSysExes.get(receivedSysExes.size() - sysExesLength + i));
        }
    }

    @Test
    public void selectSetupAtIndex_SetsCurrentSetupToSelectedSetup() {
        performanceManager.loadSetList();
        performanceManager.selectSetupAtIndex(1);
        Assert.assertEquals(setups.get(1), performanceManager.getCurrentSetup());
        assertLastSysExesReceivedWereFromSetup(setups.get(1));
    }

    @Test
    public void toggleSolo_WhenInSetupMode_SetsCurrentSetupToToggledSolo() {
        performanceManager.loadSetList();
        performanceManager.toggleSolo(solos.get(0));
        Assert.assertEquals(solos.get(0), performanceManager.getCurrentSetup());
        assertLastSysExesReceivedWereFromSetup(solos.get(0));
    }

    @Test
    public void toggleSolo_WhenInSoloMode_AndToggledSoloSelected_SetsCurrentSetupBackToPreviousSetup() {
        performanceManager.loadSetList();
        performanceManager.selectSetup(setups.get(1));
        performanceManager.toggleSolo(solos.get(0));
        performanceManager.toggleSolo(solos.get(0));
        Assert.assertEquals(setups.get(1), performanceManager.getCurrentSetup());
        assertLastSysExesReceivedWereFromSetup(setups.get(1));
    }

    @Test
    public void toggleSolo_WhenInSoloMode_AndDifferentSoloSelected_SetsCurrentSetupToToggledSolo() {
        performanceManager.loadSetList();
        performanceManager.selectSetup(setups.get(1));
        performanceManager.toggleSolo(solos.get(0));
        performanceManager.toggleSolo(solos.get(1));
        Assert.assertEquals(solos.get(1), performanceManager.getCurrentSetup());
        assertLastSysExesReceivedWereFromSetup(solos.get(1));
    }

    @Test
    public void footSwitchPressed_WhenInSetupMode_SetsCurrentSetupToNextSetupInSetList() {
        performanceManager.loadSetList();
        performanceManager.footSwitchPressed();
        Assert.assertEquals(setups.get(1), performanceManager.getCurrentSetup());
        assertLastSysExesReceivedWereFromSetup(setups.get(1));
    }

    @Test
    public void footSwitchPressed_WhenInSoloMode_SetsCurrentSetupBackToPreviousSetup() {
        performanceManager.loadSetList();
        performanceManager.toggleSolo(solos.get(1));
        performanceManager.footSwitchPressed();
        Assert.assertEquals(setups.get(0), performanceManager.getCurrentSetup());
        assertLastSysExesReceivedWereFromSetup(setups.get(0));
    }

    @Test
    public void syncCurrentSetup_OverwritesCurrentSetupWithSetupFromMidiDevice() {
        performanceManager.loadSetList();
        performanceManager.selectSetupAtIndex(1);
        performanceManager.syncCurrentSetup();
        Assert.assertEquals(midiDevice.currentSetupSysExMessages, setups.get(1).getSysExMessages());
    }
}