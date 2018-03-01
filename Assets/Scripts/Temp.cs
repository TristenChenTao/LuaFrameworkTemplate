using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using FairyGUI;
public class Temp : MonoBehaviour {

	// Use this for initialization
	GList b;
	GSlider s;
	GTextField textField;
	GComponent gComponent;
	GComponent aObject;
	void Start () {
	string a =	SystemInfo.deviceUniqueIdentifier;
	b.scrollPane.onPullUpRelease.Set (()=>{});
	if (Application.internetReachability == NetworkReachability.NotReachable)
	{	
		b.Dispose();
		
	}
	 float x  = textField.textWidth;
aObject.draggable = true;
	aObject.onDragStart.Add(onDragStart);
	DragDropManager.inst.dragAgent.onDragEnd.Add(onDragEnd);
	

	
	}
	void onDragEnd(EventContext context){
		GComponent a = (GComponent)context.sender;
		print("x的位置"+a.x);
		print("y的位置"+a.y);

	}
	void onDragStart(EventContext context)
{
    //取消掉源拖动
    context.PreventDefault();
	string icon = "ui://gamedesk/u1789";
	string userData = ""; 
    //icon是这个对象的替身图片，userData可以是任意数据，底层不作解析。context.data是手指的id。
    DragDropManager.inst.StartDrag(null, icon, userData, (int)context.data);
}
	
	// Update is called once per frame
	void Update () {
		
	}
}
