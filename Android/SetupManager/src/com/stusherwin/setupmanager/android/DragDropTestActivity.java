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

import java.util.Date;

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
    }
}