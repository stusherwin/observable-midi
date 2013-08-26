package com.stusherwin.setupmanager.core;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

public interface FileStreamProvider {
    public InputStream getInputStream() throws IOException;
    public OutputStream getOutputStream() throws FileNotFoundException;
}