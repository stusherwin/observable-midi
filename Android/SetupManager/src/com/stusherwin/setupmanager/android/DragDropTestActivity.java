package com.stusherwin.setupmanager.android;

import android.app.Activity;
import android.content.ClipData;
import android.os.Bundle;
import android.util.Log;
import android.view.DragEvent;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.*;
import com.stusherwin.setupmanager.R;

public class DragDropTestActivity extends Activity {
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);

        setContentView(R.layout.activity_drag_drop_test);

        final ListView left = (ListView)findViewById(R.id.listViewLeft);
        left.setId(123);

        final ListView right = (ListView)findViewById(R.id.listViewRight);
        right.setId(456);

        ListItem[] leftVals = new ListItem[30];
        for(int i=0; i<30; i++) {
            leftVals[i] = new ListItem(new Character((char)((int)'A' + i)).toString());
        }

        ListItem[] rightVals = new ListItem[30];
        for(int i=0; i<30; i++) {
            rightVals[i] = new ListItem(new Character((char)((int)leftVals[29].text.charAt(0) + 1 + i)).toString());
        }

        final DragDropArrayAdapter leftAdapter = new DragDropArrayAdapter(DragDropTestActivity.this, leftVals);
        final DragDropArrayAdapter rightAdapter = new DragDropArrayAdapter(DragDropTestActivity.this, rightVals);

        left.setAdapter(leftAdapter);
        right.setAdapter(rightAdapter);

        AdapterView.OnItemLongClickListener startDrag = new AdapterView.OnItemLongClickListener() {
            @Override
            public boolean onItemLongClick(AdapterView<?> adapterView, View view, int i, long l) {
                DragDropArrayAdapter adapter = (DragDropArrayAdapter) adapterView.getAdapter();
                ListItem item = (ListItem)adapterView.getItemAtPosition(adapterView.getPositionForView(view));

                view.startDrag(ClipData.newPlainText("", ""), new View.DragShadowBuilder(view), new ListItemDragDropInfo(item, adapter), 0);
                adapter.remove(item);
                return true;
            }
        };

        left.setOnItemLongClickListener(startDrag);
        right.setOnItemLongClickListener(startDrag);

        left.setOnDragListener(new ListItemDragListener(left));
        right.setOnDragListener(new ListItemDragListener(right));
    }

    private class ListItemDragDropInfo {
        public DragDropArrayAdapter sourceAdapter;
        public ListItem item;

        public ListItemDragDropInfo(ListItem item, DragDropArrayAdapter sourceAdapter) {
            this.item = item;
            this.sourceAdapter = sourceAdapter;
        }
    }

    private class ListItemDragListener implements View.OnDragListener {
        private ListView list;
        private DragDropArrayAdapter adapter;

        public ListItemDragListener(ListView list) {
            this.list = list;
            this.adapter = (DragDropArrayAdapter) list.getAdapter();
        }

        private int currentPosition;
        private ListItem placeholder = new ListItem("");

        @Override
        public boolean onDrag(android.view.View view, DragEvent dragEvent) {
            if(!(dragEvent.getLocalState() instanceof ListItemDragDropInfo)) return false;

            ListItemDragDropInfo command = (ListItemDragDropInfo)dragEvent.getLocalState();
            float x = dragEvent.getX();
            float y = dragEvent.getY();
            int position = list.pointToPosition((int)x, (int)y);
            String listId = ((Integer) list.getId()).toString();

            switch(dragEvent.getAction()) {
                case DragEvent.ACTION_DRAG_STARTED:
                    currentPosition = -1;
                    break;
                case DragEvent.ACTION_DROP:
                    Log.d(listId, "ACTION_DROP");
                    if(currentPosition > -1)
                    {
                        this.adapter.remove(placeholder);
                        adapter.insert(command.item, currentPosition);
                    }
                    break;
                case DragEvent.ACTION_DRAG_ENTERED:
                    Log.d(listId, "ACTION_DRAG_ENTERED");
                    if(position > -1) {
                        if(adapter == command.sourceAdapter) {
                            adapter.remove(command.item);
                        }
                        else {
                            adapter.insert(placeholder, position);
                        }
                    }
                    break;
                case DragEvent.ACTION_DRAG_EXITED:
                    Log.d(listId, "ACTION_DRAG_EXITED");
                    if(currentPosition > -1)
                        adapter.remove(placeholder);
                    break;
                case DragEvent.ACTION_DRAG_LOCATION:
                    Log.d(listId, "ACTION_DRAG_LOCATION");

                    if(position != currentPosition) {
                        if(currentPosition > -1 && currentPosition < adapter.getCount()) {
                            adapter.remove(placeholder);
                        }
                        if(position < adapter.getCount() && position > -1) {
                            adapter.insert(placeholder, position);
                        }
                        currentPosition = position;
                    }

                    if(currentPosition == list.getLastVisiblePosition()) {
                        list.smoothScrollToPosition(list.getLastVisiblePosition() + 1);
                    } else if(currentPosition == list.getFirstVisiblePosition()) {
                        list.smoothScrollToPosition(list.getFirstVisiblePosition() - 1);
                    }

                    break;
                case DragEvent.ACTION_DRAG_ENDED:
                    Log.d(listId, "ACTION_DRAG_ENDED");
                    break;
            }

            return true;
        }
    }

    public class ListItem {
        public String text;

        public ListItem(String text) {
            this.text = text;
        }

        @Override
        public String toString() {
            return text;
        }
    }
}