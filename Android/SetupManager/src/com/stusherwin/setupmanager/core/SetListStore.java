package com.stusherwin.setupmanager.core;

public interface SetListStore {
    SetList retrieve();

    void store(SetList _setList);
}
