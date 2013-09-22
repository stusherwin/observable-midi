package com.stusherwin.setupmanager.android;

import android.content.Context;
import android.widget.ArrayAdapter;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class DragDropArrayAdapter extends ArrayAdapter<ListItem> {
    public DragDropArrayAdapter( Context context, ListItem[] initialItems) {
        this(context, new ArrayList<ListItem>(Arrays.asList(initialItems)));
    }

    public DragDropArrayAdapter( Context context, List<ListItem> items) {
        super(context, android.R.layout.simple_list_item_1, items);
    }
}
