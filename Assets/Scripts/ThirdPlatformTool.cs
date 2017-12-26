using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cn.sharesdk.unity3d;

public class ThirdPlatformTool {
	// Use this for initialization
	public static ShareSDK _SSDK;

	public static void Authorize (int type) {
		
		if(_SSDK == null) {
			GameObject shareSDKObject = GameObject.FindWithTag("ShareSDK");
        	_SSDK = shareSDKObject.GetComponent<ShareSDK>();
			_SSDK.authHandler = OnAuthResultHandler;
			_SSDK.shareHandler = OnShareResultHandler;
			_SSDK.showUserHandler = OnGetUserInfoResultHandler;
		}

		_SSDK.Authorize(PlatformType.WeChat);
	}
	
	public static void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			if (result != null && result.Count > 0) {
				Debug.Log ("authorize success !" + "Platform :" + type + "result:" + MiniJSON.jsonEncode(result));
			} else {
				Debug.Log ("authorize success !" + "Platform :" + type);
			}
		}
		else if (state == ResponseState.Fail)
		{
			#if UNITY_ANDROID
			Debug.Log ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
			#endif
		}
		else if (state == ResponseState.Cancel) 
		{
			Debug.Log ("cancel !");
		}
	}
	
	public static void OnGetUserInfoResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result) {
		if (state == ResponseState.Success) {
			Debug.Log ("get user info result :");
			Debug.Log (MiniJSON.jsonEncode(result));
			Debug.Log ("AuthInfo:" + MiniJSON.jsonEncode (_SSDK.GetAuthInfo (PlatformType.QQ)));
			Debug.Log ("Get userInfo success !Platform :" + type );
		}
		else if (state == ResponseState.Fail) {
			#if UNITY_ANDROID
			Debug.Log ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
			#endif
		}
		else if (state == ResponseState.Cancel) {
			Debug.Log ("cancel !");
		}
	}
	
	public static void OnShareResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success)
		{
			Debug.Log ("share successfully - share result :");
			Debug.Log (MiniJSON.jsonEncode(result));
		}
		else if (state == ResponseState.Fail)
		{
			#if UNITY_ANDROID
			Debug.Log ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
			#endif
		}
		else if (state == ResponseState.Cancel) 
		{
			Debug.Log ("cancel !");
		}
	}

}
