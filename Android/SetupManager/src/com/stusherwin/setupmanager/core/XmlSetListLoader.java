package com.stusherwin.setupmanager.core;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.Formatter;
import java.util.List;
import java.util.Stack;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlSerializer;

import android.util.Xml;

public class XmlSetListLoader {
private static final String namespace = null;
	
	public SetList load(InputStream in) 
			throws XmlPullParserException, IOException {
		
		try {
            XmlPullParser parser = Xml.newPullParser();
            parser.setFeature(XmlPullParser.FEATURE_PROCESS_NAMESPACES, false);
            parser.setInput(in, null);
            parser.nextTag();
            
            ArrayList<Setup> setups = new ArrayList<Setup>();
            ArrayList<Setup> solos = new ArrayList<Setup>();
            
            int eventType = parser.getEventType();
            Stack<String> currentTag = new Stack<String>();
            String name = null;
            ArrayList<SysExMessage> msgs = null;
            
            while (eventType != XmlPullParser.END_DOCUMENT) {
                if (eventType == XmlPullParser.START_TAG) {
                	currentTag.push(parser.getName());
                    if("setup".equals(currentTag.peek()) || "solo".equals(currentTag.peek())) {
                    	msgs = new ArrayList<SysExMessage>();
                    	name = parser.getAttributeValue(namespace, "name");
                    }
                } else if (eventType == XmlPullParser.TEXT) {
                    if ("sysex".equals(currentTag.peek())) {
                        msgs.add(loadSysExMessage(parser.getText()));
                    }
                } else if (eventType == XmlPullParser.END_TAG) {
                    if ("setup".equals(currentTag.peek())) {
                    	setups.add(new Setup(name, name, msgs));
                    }
                    else if ("solo".equals(currentTag.peek())) {
                    	solos.add(new Setup(name, name, msgs));
                    }
                    currentTag.pop();
                }
                eventType = parser.next();
            }
            
            return new SetList(setups, solos);
            
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

	public void write(SetList _setList, OutputStream out) throws IOException {
		try {
			XmlSerializer serializer = Xml.newSerializer();
			serializer.setOutput(out, "UTF-8");
			serializer.startDocument("UTF-8", true);
			serializer.startTag("", "setups");
			for(Setup setup : _setList.getSetups()) {
				writeSetup(serializer, setup, "setup");
			}
			for(Setup solo : _setList.getSolos()) {
				writeSetup(serializer, solo, "solo");
			}
			serializer.endTag("", "setups");
		} finally {
			out.close();
		}
	}
	
	private void writeSetup(XmlSerializer serializer, Setup setup, String tagName ) throws IllegalArgumentException, IllegalStateException, IOException {
		serializer.startTag("", tagName);
		for(SysExMessage sysex : setup.getSysExMessages()) {
			serializer.startTag("", "sysex");
			byte[] bytes = sysex.getBytes();

			StringBuilder sb = new StringBuilder(bytes.length * 2 + bytes.length - 1);  
			  
		    Formatter formatter = new Formatter(sb);  
		    for (int i = 0; i < bytes.length; i++) {  
		        formatter.format("%2X", bytes[i]);
		        sb.append(" ");
		    }  
		    serializer.text(sb.toString());
			serializer.endTag("", "sysex");
		}
		serializer.endTag("", tagName);
	}
}