package com.stusherwin.setupmanager.midi;

import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import jp.kshoji.driver.midi.device.MidiInputDevice;
import jp.kshoji.driver.midi.device.MidiOutputDevice;
import jp.kshoji.driver.midi.listener.OnMidiDeviceAttachedListener;
import jp.kshoji.driver.midi.listener.OnMidiDeviceDetachedListener;
import jp.kshoji.driver.midi.listener.OnMidiInputEventListener;
import jp.kshoji.driver.midi.thread.MidiDeviceConnectionWatcher;
import jp.kshoji.driver.midi.util.Constants;
import jp.kshoji.driver.midi.util.UsbMidiDeviceUtils;
import jp.kshoji.driver.usb.util.DeviceFilter;
import android.content.Context;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbManager;
import android.os.Handler;
import android.os.Message;
import android.os.Handler.Callback;
import android.util.Log;

public class MidiManager implements OnMidiInputEventListener {
	final class OnMidiDeviceAttachedListenerImpl implements OnMidiDeviceAttachedListener {
		private final UsbManager usbManager;
		private final Context context;
		
		public OnMidiDeviceAttachedListenerImpl(Context context, UsbManager usbManager) {
			this.context = context;
			this.usbManager = usbManager;
		}
		
		@Override
		public synchronized void onDeviceAttached(UsbDevice attachedDevice) {
			// these fields are null; when this event fired while Activity destroying.
			if (midiInputDevices == null || midiOutputDevices == null || deviceConnections == null) {
				// nothing to do
				return;
			}
			
			deviceConnectionWatcher.notifyDeviceGranted();

			UsbDeviceConnection deviceConnection = usbManager.openDevice(attachedDevice);
			if (deviceConnection == null) {
				return;
			}

			deviceConnections.put(attachedDevice, deviceConnection);
			
			List<DeviceFilter> deviceFilters = DeviceFilter.getDeviceFilters(this.context);

			Set<MidiInputDevice> foundInputDevices = UsbMidiDeviceUtils.findMidiInputDevices(attachedDevice, deviceConnection, deviceFilters, MidiManager.this);
			for (MidiInputDevice midiInputDevice : foundInputDevices) {
				try {
					Set<MidiInputDevice> inputDevices = midiInputDevices.get(attachedDevice);
					if (inputDevices == null) {
						inputDevices = new HashSet<MidiInputDevice>();
					}
					inputDevices.add(midiInputDevice);
					midiInputDevices.put(attachedDevice, inputDevices);
				} catch (IllegalArgumentException iae) {
					Log.i(Constants.TAG, "This device didn't have any input endpoints.", iae);
				}
			}
			
			Set<MidiOutputDevice> foundOutputDevices = UsbMidiDeviceUtils.findMidiOutputDevices(attachedDevice, deviceConnection, deviceFilters);
			for (MidiOutputDevice midiOutputDevice : foundOutputDevices) {
				try {
					Set<MidiOutputDevice> outputDevices = midiOutputDevices.get(attachedDevice);
					if (outputDevices == null) {
						outputDevices = new HashSet<MidiOutputDevice>();
					}
					outputDevices.add(midiOutputDevice);
					midiOutputDevices.put(attachedDevice, outputDevices);
				} catch (IllegalArgumentException iae) {
					Log.i(Constants.TAG, "This device didn't have any output endpoints.", iae);
				}
			}
			
			Log.d(Constants.TAG, "Device " + attachedDevice.getDeviceName() + " has been attached.");
		}
	}

	final class OnMidiDeviceDetachedListenerImpl implements OnMidiDeviceDetachedListener {
		@Override
		public synchronized void onDeviceDetached(UsbDevice detachedDevice) {
			// these fields are null; when this event fired while Activity destroying.
			if (midiInputDevices == null || midiOutputDevices == null || deviceConnections == null) {
				// nothing to do
				return;
			}
			
			// Stop input device's thread.
			Set<MidiInputDevice> inputDevices = midiInputDevices.get(detachedDevice);
			if (inputDevices != null && inputDevices.size() > 0) {
				for (MidiInputDevice inputDevice : inputDevices) {
					if (inputDevice != null) {
						inputDevice.stop();
					}
				}
				midiInputDevices.remove(detachedDevice);
			}
			
			Set<MidiOutputDevice> outputDevices = midiOutputDevices.get(detachedDevice);
			if (outputDevices != null) {
				for (MidiOutputDevice outputDevice : outputDevices) {
					if (outputDevice != null) {
						outputDevice.stop();
					}
				}
				midiOutputDevices.remove(detachedDevice);
			}

			UsbDeviceConnection deviceConnection = deviceConnections.get(detachedDevice);
			if (deviceConnection != null) {
				deviceConnection.close();
				
				deviceConnections.remove(detachedDevice);
			}
			
			Log.d(Constants.TAG, "Device " + detachedDevice.getDeviceName() + " has been detached.");
			
			Message message = new Message();
			message.obj = detachedDevice;
			deviceDetachedHandler.sendMessage(message);
		}
	}

	Map<UsbDevice, UsbDeviceConnection> deviceConnections = null;
	Map<UsbDevice, Set<MidiInputDevice>> midiInputDevices = null;
	Map<UsbDevice, Set<MidiOutputDevice>> midiOutputDevices = null;
	OnMidiDeviceAttachedListener deviceAttachedListener = null;
	OnMidiDeviceDetachedListener deviceDetachedListener = null;
	Handler deviceDetachedHandler = null;
	MidiDeviceConnectionWatcher deviceConnectionWatcher = null;
	Set<OnMidiInputEventListener> inputEventListeners = null;

	public void initialize(Context context) {
		inputEventListeners = new HashSet<OnMidiInputEventListener>();
		deviceConnections = new HashMap<UsbDevice, UsbDeviceConnection>();
		midiInputDevices = new HashMap<UsbDevice, Set<MidiInputDevice>>();
		midiOutputDevices = new HashMap<UsbDevice, Set<MidiOutputDevice>>();
		
		UsbManager usbManager = (UsbManager) context.getSystemService(Context.USB_SERVICE);
		deviceAttachedListener = new OnMidiDeviceAttachedListenerImpl(context, usbManager);
		deviceDetachedListener = new OnMidiDeviceDetachedListenerImpl();
		
		deviceDetachedHandler = new Handler(new Callback() {
			@Override
			public boolean handleMessage(Message msg) {
				Log.i(Constants.TAG, "(handleMessage) detached device:" + msg.obj);
				UsbDevice usbDevice = (UsbDevice) msg.obj;
				return true;
			}
		});

		deviceConnectionWatcher = new MidiDeviceConnectionWatcher(context, usbManager, deviceAttachedListener, deviceDetachedListener);
	}
	
	public void addInputListener(OnMidiInputEventListener listener) {
		inputEventListeners.add(listener);
	}
	
	public void destroy() {		
		deviceConnectionWatcher.stop();
		deviceConnectionWatcher = null;
		
		if (midiInputDevices != null) {
			for (Set<MidiInputDevice> inputDevices : midiInputDevices.values()) {
				if (inputDevices != null) {
					for (MidiInputDevice inputDevice : inputDevices) {
						if (inputDevice != null) {
							inputDevice.stop();
						}
					}
				}
			}
			
			midiInputDevices.clear();
		}
		midiInputDevices = null;
		
		if (midiOutputDevices != null) {
			midiOutputDevices.clear();
		}
		midiOutputDevices = null;
		
		deviceConnections = null;
	}
	
	public final Set<UsbDevice> getConnectedUsbDevices() {
		if (deviceConnectionWatcher != null) {
			deviceConnectionWatcher.checkConnectedDevicesImmediately();
		}
		if (deviceConnections != null) {
			return Collections.unmodifiableSet(deviceConnections.keySet());
		}
		
		return Collections.unmodifiableSet(new HashSet<UsbDevice>());
	}
	
	/**
	 * Get MIDI output device, if available.
	 * 
	 * @param usbDevice
	 * @return {@link Set<MidiOutputDevice>}
	 */
	public final Set<MidiOutputDevice> getMidiOutputDevices(UsbDevice usbDevice) {
		if (deviceConnectionWatcher != null) {
			deviceConnectionWatcher.checkConnectedDevicesImmediately();
		}
		if (midiOutputDevices != null) {
			return Collections.unmodifiableSet(midiOutputDevices.get(usbDevice));
		}
		
		return Collections.unmodifiableSet(new HashSet<MidiOutputDevice>());
	}

	@Override
	public void onMidiMiscellaneousFunctionCodes(MidiInputDevice sender,
			int cable, int byte1, int byte2, int byte3) {
		
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiMiscellaneousFunctionCodes(sender, cable, byte1, byte2, byte3);
		}		
	}

	@Override
	public void onMidiCableEvents(MidiInputDevice sender, int cable, int byte1,
			int byte2, int byte3) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiCableEvents(sender, cable, byte1, byte2, byte3);
		}
	}

	@Override
	public void onMidiSystemCommonMessage(MidiInputDevice sender, int cable,
			byte[] bytes) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiSystemCommonMessage(sender, cable, bytes);	
		}
	}

	@Override
	public void onMidiSystemExclusive(MidiInputDevice sender, int cable,
			byte[] systemExclusive) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiSystemExclusive(sender, cable, systemExclusive);
		}
	}

	@Override
	public void onMidiNoteOff(MidiInputDevice sender, int cable, int channel,
			int note, int velocity) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiNoteOff(sender, cable, channel, note, velocity);
		}
	}

	@Override
	public void onMidiNoteOn(MidiInputDevice sender, int cable, int channel,
			int note, int velocity) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiNoteOn(sender, cable, channel, note, velocity);
		}
	}

	@Override
	public void onMidiPolyphonicAftertouch(MidiInputDevice sender, int cable,
			int channel, int note, int pressure) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiPolyphonicAftertouch(sender, cable, channel, note, pressure);
		}
	}

	@Override
	public void onMidiControlChange(MidiInputDevice sender, int cable,
			int channel, int function, int value) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiControlChange(sender, cable, channel, function, value);
		}
	}

	@Override
	public void onMidiProgramChange(MidiInputDevice sender, int cable,
			int channel, int program) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiProgramChange(sender, cable, channel, program);
		}
	}

	@Override
	public void onMidiChannelAftertouch(MidiInputDevice sender, int cable,
			int channel, int pressure) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiChannelAftertouch(sender, cable, channel, pressure);
		}
	}

	@Override
	public void onMidiPitchWheel(MidiInputDevice sender, int cable,
			int channel, int amount) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiPitchWheel(sender, cable, channel, amount);
		}
	}

	@Override
	public void onMidiSingleByte(MidiInputDevice sender, int cable, int byte1) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiSingleByte(sender, cable, byte1);			
		}
	}

	@Override
	public void onMidiRPNReceived(MidiInputDevice sender, int cable,
			int channel, int function, int valueMSB, int valueLSB) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiRPNReceived(sender, cable, channel, function, valueMSB, valueLSB);
		}
	}

	@Override
	public void onMidiNRPNReceived(MidiInputDevice sender, int cable,
			int channel, int function, int valueMSB, int valueLSB) {
		for(OnMidiInputEventListener listener: inputEventListeners) {
			listener.onMidiNRPNReceived(sender, cable, channel, function, valueMSB, valueLSB);
		}
	}
	
	public void sendMidiNoteOn(int cable, int channel, int note, int velocity) {
		for(Set<MidiOutputDevice> devices : midiOutputDevices.values()) {
			for(MidiOutputDevice device : devices) {
				device.sendMidiNoteOn(cable, channel, note, velocity);
			}
		}
	}
	
	public void sendMidiNoteOff(int cable, int channel, int note, int velocity) {
		for(Set<MidiOutputDevice> devices : midiOutputDevices.values()) {
			for(MidiOutputDevice device : devices) {
				device.sendMidiNoteOff(cable, channel, note, velocity);
			}
		}
	}
	
	public void sendMidiSystemExclusive(int cable, byte[] systemExclusive) {
		for(Set<MidiOutputDevice> devices : midiOutputDevices.values()) {
			for(MidiOutputDevice device : devices) {
				device.sendMidiSystemExclusive(cable, systemExclusive);
			}
		}
	}
}