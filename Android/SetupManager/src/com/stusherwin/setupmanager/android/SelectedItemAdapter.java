package com.stusherwin.setupmanager.android;

import java.util.List;

import com.stusherwin.setupmanager.R;
import com.stusherwin.setupmanager.core.Setup;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

public class SelectedItemAdapter<T> extends BaseAdapter
{
    private final Context context;
	private List<T> items;
	private int selectedPosition;

    public SelectedItemAdapter( Context context, List<T> items ) {
        this.context = context;
        this.items = items;
		this.selectedPosition = 0;
    }
	
	public void selectItemAtPosition(int position) {
		this.selectedPosition = Math.min(this.items.size() - 1, Math.max(0, position));
		
		this.notifyDataSetChanged();
	}

    public void selectItem(T item) {
        selectItemAtPosition(items.indexOf(item));
    }

    public int getSelectedItemPosition() {
        return selectedPosition;
    }
     
    @Override
    public int getCount() {
        return this.items.size();
    }
 
    @Override
    public Object getItem( int position ) {
        return this.items.get( position );
    }
 
    @Override
    public long getItemId( int position ) {
        return getItem( position ).hashCode();
    }
 
    @Override
    public View getView( int position, View convertView, ViewGroup parent )
    {
        /*
         * Please note that while this code works it is somewhat inefficient
         * and may result in some jerky scrolling. Please read the article
         * which explains this code at http://blog.stylingandroid.com/archives/623
         * for further explanation and base any production code on the later, 
         * more efficient examples.
         */
    	int layoutId = position == this.selectedPosition ? R.layout.selected_list_item : R.layout.list_item;
    	
        LayoutInflater inflater = (LayoutInflater) 
            context.getSystemService( Context.LAYOUT_INFLATER_SERVICE );
        View v = inflater.inflate( layoutId, parent, false );
        final String item = getItem( position ).toString();
        TextView tv = (TextView)v.findViewById( R.id.text );
        tv.setText( item );
        return v;
    }
}