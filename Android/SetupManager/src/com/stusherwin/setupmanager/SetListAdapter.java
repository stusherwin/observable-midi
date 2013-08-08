package com.stusherwin.setupmanager;

import java.util.List;

import com.stusherwin.setupmanager.core.Setup;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

public class SetListAdapter extends BaseAdapter
{
    private final Context context;
	private List<Setup> setups;
	private int selectedSetupPosition;
     
    public SetListAdapter( Context context, List<Setup> setups )
    {
        this.context = context;
        this.setups = setups;
		this.selectedSetupPosition = 0;
    }
	
	public int getSelectedSetupPosition() {
		return this.selectedSetupPosition;
	}
	
	public Setup getSelectedSetup() {
		return this.setups.get(this.selectedSetupPosition);
	}
	
	public void selectSetupAtPosition(int position) {
		this.selectedSetupPosition = Math.min(this.setups.size() - 1, Math.max(0, position));
		
		this.notifyDataSetChanged();
	}
     
    @Override
    public int getCount()
    {
        return this.setups.size();
    }
 
    @Override
    public Object getItem( int position )
    {
        return this.setups.get( position );
    }
 
    @Override
    public long getItemId( int position )
    {
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
    	int layoutId = position == this.selectedSetupPosition ? R.layout.selected_list_item : R.layout.list_item;
    	
        LayoutInflater inflater = (LayoutInflater) 
            context.getSystemService( Context.LAYOUT_INFLATER_SERVICE );
        View v = inflater.inflate( layoutId, parent, false );
        final String item = getItem( position ).toString();
        TextView tv = (TextView)v.findViewById( R.id.text );
        tv.setText( item );
        return v;
    }
}