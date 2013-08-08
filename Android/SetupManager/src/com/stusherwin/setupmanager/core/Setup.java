package com.stusherwin.setupmanager.core;

import java.util.List;

public class Setup {
	private String _name;
	public String getName() {
		return _name;
	};
	
	private String _rdName;
	public String getRDName() {
		return _rdName;
	};
	
	private List<SysExMessage> _sysExMessages;
	public List<SysExMessage> getSysExMessages() {
		return _sysExMessages;
	};
	
	public Setup(String name, String rdName, List<SysExMessage> sysExMessages) {
		_name = name;
		_rdName = rdName;
		_sysExMessages = sysExMessages;
	}
	
	@Override
	public String toString() {
		return getName();
	}

    public static Setup load(List<SysExMessage> sysExMessages)
    {
    	SysExMessage nameMessage = null;
    	for(SysExMessage m : sysExMessages) {
    		if(m.isNameMessage()) {
    			nameMessage = m;
    			break;
    		}
    	}

        return new Setup(nameMessage.getName(), nameMessage.getName(), sysExMessages);
    }
}