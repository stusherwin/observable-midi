package com.stusherwin.setupmanager.core;

import java.util.ArrayList;
import java.util.List;

public class SetList {
    List<Setup> _setups;
    List<Setup> _solos;

    public SetList(List<Setup> setups, List<Setup> solos) {
    	_setups = setups;
    	_solos = solos;
    }
    
    public List<Setup> getSetups() {
    	return _setups;
    }
    
    public List<Setup> getSolos() {
    	return _solos;
    }

    public Setup getNextSetup(Setup currentSetup) {
        int currentIndex = _setups.indexOf(currentSetup);
        int nextIndex = Math.min(_setups.size() - 1, Math.max(0, currentIndex + 1));
        return _setups.get(nextIndex);
    }

    public Setup getSetupAtIndex(int index) {
        return _setups.get(index);
    }

    public Setup getSoloAtIndex(int index) {
        return _solos.get(index);
    }

    public int getSetupIndex(Setup setup) {
        return _setups.indexOf(setup);
    }
}