package com.stusherwin.setupmanager.test.core;

import com.stusherwin.setupmanager.core.Setup;
import com.stusherwin.setupmanager.core.SetupChangeListener;
import com.stusherwin.setupmanager.core.SysExMessage;

import java.util.ArrayList;

public class FakeSetupChangeListener implements SetupChangeListener {
    private Setup initialSetup = new Setup("Initial", "Initial", new ArrayList<SysExMessage>());
    public Setup latestSelectedSetup = initialSetup;
    public Setup latestSelectedSolo = initialSetup;

    @Override
    public void selectedSetupChanged(Setup selectedSetup) {
        latestSelectedSetup = selectedSetup;
    }

    @Override
    public void selectedSoloChanged(Setup selectedSolo) {
        latestSelectedSolo = selectedSolo;
    }

    public void startListening() {
        latestSelectedSetup = initialSetup;
        latestSelectedSolo = initialSetup;
    }

    public boolean selectedSetupWasChanged() {
        return latestSelectedSetup != initialSetup;
    }

    public boolean selectedSoloWasChanged() {
        return latestSelectedSolo != initialSetup;
    }
}
