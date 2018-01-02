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


	enum ThirdPlatformType : int {
		WeChat = 1,
		QQ = 2,
		Weibo = 3
    };

	public static void Authorize (int type,LuaFunction func = null) {
		ConfigSSDK();

		PlatformType finaltype = ThirdPlatformTool.fromInt(type);
		_SSDK.Authorize(finaltype);
		_AuthorLuaFunc = func;
	}	
	public static void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		int platformType = ThirdPlatformTool.fromPlatformType(type);

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

			_AuthorLuaFunc.Call((int)AuthResponseState.Fail, "授权失败", platformType);
		}
		else if (state == ResponseState.Cancel) 
		{
			Debug.Log ("cancel !");

			_AuthorLuaFunc.Call((int)AuthResponseState.Cancel, "授权取消", platformType);
		}
	}
	
	public static void OnGetUserInfoResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result) {

		int platformType = ThirdPlatformTool.fromPlatformType(type);

		if (state == ResponseState.Success) {
			Debug.Log ("get user info result :");
			Debug.Log (MiniJSON.jsonEncode(result));
			Debug.Log ("AuthInfo:" + MiniJSON.jsonEncode (_SSDK.GetAuthInfo (type)));
			Debug.Log ("Get userInfo success !Platform :" + type );
			
			string userInfo = MiniJSON.jsonEncode(result);
			string authInfo = MiniJSON.jsonEncode(_SSDK.GetAuthInfo (type));

			_AuthorLuaFunc.Call((int)AuthResponseState.Sucess, "授权成功", platformType, userInfo,authInfo);
		}
		else if (state == ResponseState.Fail) {
			#if UNITY_ANDROID
			Debug.Log ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
			#endif

			_AuthorLuaFunc.Call((int)AuthResponseState.Fail, "获取用户信息失败", platformType);
		}
		else if (state == ResponseState.Cancel) {
			Debug.Log ("cancel !");

			_AuthorLuaFunc.Call((int)AuthResponseState.Cancel, "取消获取用户信息", platformType);
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


	private static PlatformType fromInt(int type) {
		
		ThirdPlatformType thirdPlatformType = (ThirdPlatformType)type;

		PlatformType finaltype = PlatformType.WeChat;

		if (thirdPlatformType == ThirdPlatformType.WeChat) {
			finaltype = PlatformType.WeChat;
		}
		else if (thirdPlatformType == ThirdPlatformType.QQ) {
			finaltype = PlatformType.QQ;
		}
		else if (thirdPlatformType == ThirdPlatformType.Weibo) {
			finaltype = PlatformType.SinaWeibo;
		}

		return finaltype;
	}

	private static int fromPlatformType(PlatformType type) {

		int finaltype = (int)ThirdPlatformType.WeChat;

		if (type == PlatformType.WeChat) {
			finaltype = (int)ThirdPlatformType.WeChat;
		}
		else if (type == PlatformType.QQ) {
			finaltype = (int)ThirdPlatformType.QQ;
		}
		else if (type == PlatformType.SinaWeibo) {
			finaltype = (int)ThirdPlatformType.Weibo;
		}

		return finaltype;
	}

	private static void ConfigSSDK() {
		if(_SSDK == null) {
			GameObject shareSDKObject = GameObject.FindWithTag("ShareSDK");
        	_SSDK = shareSDKObject.GetComponent<ShareSDK>();
			_SSDK.authHandler = OnAuthResultHandler;
			_SSDK.shareHandler = OnShareResultHandler;
			_SSDK.showUserHandler = OnGetUserInfoResultHandler;
		}
	}
}
