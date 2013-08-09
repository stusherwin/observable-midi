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
}
