using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cn.sharesdk.unity3d;
using LuaInterface;

public class ThirdPlatformTool {
	// Use this for initialization
	public static ShareSDK _SSDK;

	private static LuaFunction _AuthorLuaFunc;

	enum AuthResponseState : int {
		Sucess = 1,
		Fail = 0,
		Cancel = -1
    };

	public static void Authorize (int type,LuaFunction func = null) {
		ConfigSSDK();

		_SSDK.Authorize(PlatformType.WeChat);
		_AuthorLuaFunc = func;
	}

	private static void ConfigSSDK(){
		if(_SSDK == null) {
			GameObject shareSDKObject = GameObject.FindWithTag("ShareSDK");
        	_SSDK = shareSDKObject.GetComponent<ShareSDK>();
			_SSDK.authHandler = OnAuthResultHandler;
			_SSDK.shareHandler = OnShareResultHandler;
			_SSDK.showUserHandler = OnGetUserInfoResultHandler;
		}
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

			_SSDK.GetUserInfo(type);
		}
		else if (state == ResponseState.Fail)
		{
			#if UNITY_ANDROID
				Debug.Log ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			#elif UNITY_IPHONE
				Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
			#endif

			_AuthorLuaFunc.Call((int)AuthResponseState.Fail, "授权失败");
		}
		else if (state == ResponseState.Cancel) 
		{
			Debug.Log ("cancel !");

			_AuthorLuaFunc.Call((int)AuthResponseState.Cancel, "授权取消");
		}
	}
	
	public static void OnGetUserInfoResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result) {
		if (state == ResponseState.Success) {
			Debug.Log ("get user info result :");
			Debug.Log (MiniJSON.jsonEncode(result));
			Debug.Log ("AuthInfo:" + MiniJSON.jsonEncode (_SSDK.GetAuthInfo (type)));
			Debug.Log ("Get userInfo success !Platform :" + type );
			
			string userInfo = MiniJSON.jsonEncode(result);
			string authInfo = MiniJSON.jsonEncode(_SSDK.GetAuthInfo (type));

			_AuthorLuaFunc.Call((int)AuthResponseState.Sucess, "授权成功",userInfo,authInfo);
		}
		else if (state == ResponseState.Fail) {
			#if UNITY_ANDROID
			Debug.Log ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
			#endif

			_AuthorLuaFunc.Call((int)AuthResponseState.Fail, "获取用户信息失败");
		}
		else if (state == ResponseState.Cancel) {
			Debug.Log ("cancel !");

			_AuthorLuaFunc.Call((int)AuthResponseState.Cancel, "取消获取用户信息");
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
