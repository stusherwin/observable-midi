package com.stusherwin.setupmanager.core;

import java.util.Arrays;

public class SysExMessage {
	private static final byte[] NAME_MESSAGE_ADDRESS = new byte[] { 0x10, 0x00, 0x00, 0x00 };
	private final int ADDRESS_START = 8;
	private final int ADDRESS_LENGTH = 12;
	private final int NAME_START = 12;
	private final int NAME_LENGTH = 12;

	private byte[] _bytes;
    public byte[] getBytes() { return _bytes; }

    public SysExMessage(byte[] bytes)
    {
        _bytes = bytes;
    }

    public byte[] getAddress()
    {
		return getBytes(ADDRESS_START, ADDRESS_LENGTH);
    }

    public boolean isNameMessage()
    {
        return Arrays.equals(getAddress(), NAME_MESSAGE_ADDRESS);
    }

    public String getName()
    {
        return isNameMessage() ?
        	new String(getBytes(NAME_START, NAME_LENGTH))
          : "";
    }
    
    final protected static char[] hexArray = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};
    
    public String getBytesString() {
        char[] hexChars = new char[_bytes.length * 2 + _bytes.length - 1];
        int v;
        for ( int j = 0; j < _bytes.length; j++ ) {
            v = _bytes[j] & 0xFF;
            hexChars[j * 3] = hexArray[v >>> 4];
            hexChars[j * 3 + 1] = hexArray[v & 0x0F];
            if(j < _bytes.length - 1) {
            	hexChars[j * 3 + 2] = ' '; 
            }
        }
        return new String(hexChars);
    }

	private byte[] getBytes(int start, int length) {
	    return Arrays.copyOfRange(getBytes(), start, start + length);
	}
}