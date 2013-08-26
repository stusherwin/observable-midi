package com.stusherwin.setupmanager.android;

import android.app.Activity;
import com.stusherwin.setupmanager.core.FileStreamProvider;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

public class ActivitySetListFileStreamProvider implements FileStreamProvider {
    private final Activity _assetManager;

    public ActivitySetListFileStreamProvider(Activity assetManager) {
        _assetManager = assetManager;
    }

    public InputStream getInputStream() throws IOException {
        return _assetManager.getAssets().open("setups.xml");
    }

//	private InputStream getInputStream() throws IOException {
//		File setupsFile = getFileStreamPath("setups.xml");
//		if(!setupsFile.exists()) {
//			InputStream asset = this.getAssets().open("setups.xml");
//			OutputStream out = openFileOutput("setups.xml", MODE_WORLD_READABLE);
//			try {
//				byte[] buf = new byte[1024];
//			    int len;
//			    while ((len = asset.read(buf)) > 0) {
//			        out.store(buf, 0, len);
//			    }
//			} finally {
//				if(asset != null)
//					asset.close();
//				if(out != null)
//					out.close();
//			}
//		}
//		return openFileInput("setups.xml");
//	}

    public OutputStream getOutputStream() throws FileNotFoundException {
        return _assetManager.openFileOutput("setups.xml", Activity.MODE_WORLD_READABLE);
    }
}
