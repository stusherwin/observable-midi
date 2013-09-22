package com.stusherwin.setupmanager.android;

import android.content.ClipData;
import android.content.Context;
import android.util.AttributeSet;
import android.util.Log;
import android.view.DragEvent;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import java.util.Date;

public class DragDropListView extends ListView {
    public DragDropListView(Context context, AttributeSet attrs) {
        super(context, attrs);

        setup();
    }

    public DragDropListView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);

        setup();
    }

    public DragDropListView(Context context) {
        super(context);

        setup();
    }

    private void setup() {
        setOnItemLongClickListener(new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> adapterView, View view, int i, long l) {
                ArrayAdapter adapter = (ArrayAdapter) adapterView.getAdapter();
                ListItem item = (ListItem) adapterView.getItemAtPosition(adapterView.getPositionForView(view));

                view.startDrag(ClipData.newPlainText("", ""), new View.DragShadowBuilder(view), new ListItemDragDropInfo(item, adapter), 0);
                adapter.remove(item);
                return true;
            }
        });
        setOnDragListener( new ListItemDragListener());
    }

    private class ListItemDragDropInfo {
        public ArrayAdapter sourceAdapter;
        public ListItem item;

        public ListItemDragDropInfo(ListItem item, ArrayAdapter sourceAdapter) {
            this.item = item;
            this.sourceAdapter = sourceAdapter;
        }
    }

    private class ListItemDragListener implements View.OnDragListener {
        private ListView getList() { return DragDropListView.this; }
        private ArrayAdapter getAdapter() { return (ArrayAdapter) getList().getAdapter(); }

        private int oldPosition;
        private ListItem placeholder = new ListItem("");
        private boolean oldInserting = false;
        private boolean oldScrolling = false;
        private Date lastMoved;

        @Override
        public boolean onDrag(android.view.View view, DragEvent dragEvent) {
            if(!(dragEvent.getLocalState() instanceof ListItemDragDropInfo)) return false;

            ListItemDragDropInfo command = (ListItemDragDropInfo)dragEvent.getLocalState();
            float x = dragEvent.getX();
            float y = dragEvent.getY();
            int position = getList().pointToPosition((int)x, (int)y);
            String listId = ((Integer) getList().getId()).toString();

            switch(dragEvent.getAction()) {
                case DragEvent.ACTION_DRAG_STARTED:
                    oldPosition = -1;
                    break;
                case DragEvent.ACTION_DROP:
                    //Log.d(listId, "ACTION_DROP");
                    if(oldPosition > -1)
                    {
                        getAdapter().remove(placeholder);
                        //Log.d(listId, "removed placeholder");
                        getAdapter().insert(command.item, oldPosition);
                    }
                    break;
                case DragEvent.ACTION_DRAG_ENTERED:
                    //Log.d(listId, "ACTION_DRAG_ENTERED");
                    if(position > -1) {
                        if(getAdapter() == command.sourceAdapter) {
                            getAdapter().remove(command.item);
                        }
                        else {
                            getAdapter().insert(placeholder, position);
                            //Log.d(listId, "added placeholder");
                        }
                    }
                    break;
                case DragEvent.ACTION_DRAG_EXITED:
                    //Log.d(listId, "ACTION_DRAG_EXITED");
                    if(oldPosition > -1)                  {
                        getAdapter().remove(placeholder);
                        //Log.d(listId, "removed placeholder");
                    }
                    break;
                case DragEvent.ACTION_DRAG_LOCATION:
                    //Log.d(listId, "ACTION_DRAG_LOCATION");

                    boolean inserting = oldInserting;
                    boolean scrolling = position == getList().getLastVisiblePosition() || position == getList().getFirstVisiblePosition();
                    //Log.d(listId, "scrolling " + scrolling);

                    if(position != oldPosition) {
                        //Log.d(listId, "new position");
                        lastMoved = new Date();
                        inserting = false;
                    }
                    else if(new Date().getTime() - lastMoved.getTime() > 500) {
                        inserting = true;
                    }
                    //if(inserting != oldInserting) {
                    //    Log.d(listId, "inserting now " + inserting);
                    //}
                    //if(scrolling != oldScrolling) {
                    //    Log.d(listId, "scrolling now " + scrolling);
                    //}

                    //if(scrolling) {
                    //    inserting = false;
                    //}

                    if(inserting && !oldInserting) {
                        getAdapter().insert(placeholder, position);
                        Log.d(listId, "added placeholder");
                    }
                    else if(!inserting && oldInserting){
                        getAdapter().remove(placeholder);
                        Log.d(listId, "removed placeholder");
                    }

                    if(inserting) {
                        if(position != oldPosition) {
                            getAdapter().remove(placeholder);
                            Log.d(listId, "removed placeholder");

                            if( position >= getAdapter().getCount()) {
                                getAdapter().add(placeholder);
                            }
                            else if(position > -1) {
                                getAdapter().insert(placeholder, position);
                            }
                            Log.d(listId, "added placeholder");
                        }
                    }
                    else if(scrolling) {
                        if(position == getList().getLastVisiblePosition()) {
                            getList().smoothScrollToPosition(getList().getLastVisiblePosition() + 1);
                        } else if(position == getList().getFirstVisiblePosition()) {
                            getList().smoothScrollToPosition(getList().getFirstVisiblePosition() - 1);
                        }
                    }

                    oldPosition = position;
                    oldInserting = inserting;
                    oldScrolling = scrolling;

                    break;
                case DragEvent.ACTION_DRAG_ENDED:
                    //Log.d(listId, "ACTION_DRAG_ENDED");
                    break;
            }

            return true;
        }
    }
}

