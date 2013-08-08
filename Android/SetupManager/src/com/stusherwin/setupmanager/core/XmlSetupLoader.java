package com.stusherwin.setupmanager.core;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;
import java.util.Stack;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;

import android.util.Xml;

public class XmlSetupLoader {
private static final String namespace = null;
	
	public List<Setup> load(InputStream in) 
			throws XmlPullParserException, IOException {
		
		try {
            XmlPullParser parser = Xml.newPullParser();
            parser.setFeature(XmlPullParser.FEATURE_PROCESS_NAMESPACES, false);
            parser.setInput(in, null);
            parser.nextTag();
            
            ArrayList<Setup> results = new ArrayList<Setup>();
            
            int eventType = parser.getEventType();
            Stack<String> currentTag = new Stack<String>();
            String name = null;
            ArrayList<SysExMessage> msgs = null;
            
            while (eventType != XmlPullParser.END_DOCUMENT) {
                if (eventType == XmlPullParser.START_TAG) {
                	currentTag.push(parser.getName());
                    if("setup".equals(currentTag.peek())) {
                    	msgs = new ArrayList<SysExMessage>();
                    	name = parser.getAttributeValue(namespace, "name");
                    }
                } else if (eventType == XmlPullParser.TEXT) {
                    if ("sysex".equals(currentTag.peek())) {
                        msgs.add(loadSysExMessage(parser.getText()));
                    }
                } else if (eventType == XmlPullParser.END_TAG) {
                    if ("setup".equals(currentTag.peek())) {
                        results.add(new Setup(name, name, msgs));
                    }
                    currentTag.pop();
                }
                eventType = parser.next();
            }
            
            return results;
            
        } finally {
            in.close();
        }
	}
	
	private SysExMessage loadSysExMessage(String content) {
		String[] byteStrings = content.split(" ");
		byte[] bytes = new byte[byteStrings.length];
		
		for(int i = 0; i < bytes.length; i++) {
			bytes[i] = (byte)Integer.parseInt(byteStrings[i], 16);
		}

		return new SysExMessage(bytes);
	}
}