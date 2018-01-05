using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cn.sharesdk.unity3d;
using LuaInterface;

public class ThirdPlatformTool {
	// Use this for initialization
	public static ShareSDK _SSDK;

	private static LuaFunction _AuthorLuaFunc;

	private static LuaFunction _ShareLuaFunc;

	enum ThirdResponseState : int {
		Sucess = 1,
		Fail = 0,
		Cancel = -1
    };

	enum ThirdPlatformType : int {
		WeChat = 1,
		QQ = 2,
		Weibo = 3,
		WeChatMoments = 4
    };

	public static void Authorize (int type,LuaFunction func = null) {
		ConfigSSDK();

		PlatformType finaltype = ThirdPlatformTool.fromInt(type);
		_SSDK.Authorize(finaltype);
		_AuthorLuaFunc = func;
	}
	public static void Share (int type, ShareContent content, LuaFunction func = null) {
		Debug.Log ("Start Share ");
		ConfigSSDK();

		PlatformType finaltype = ThirdPlatformTool.fromInt(type);
		 _SSDK.ShareContent(finaltype, content);
		_SSDK.ShareContent (PlatformType.WeChat, content);
			
		_ShareLuaFunc = func;
	}	
	private static void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
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

			if (_AuthorLuaFunc != null) {
				_AuthorLuaFunc.Call((int)ThirdResponseState.Fail, "授权失败", platformType);
			}
		}
		else if (state == ResponseState.Cancel) 
		{
			Debug.Log ("cancel !");
			if (_AuthorLuaFunc != null) {
				_AuthorLuaFunc.Call((int)ThirdResponseState.Cancel, "授权取消", platformType);
			}
		}
	}
	
	private static void OnGetUserInfoResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result) {

		int platformType = ThirdPlatformTool.fromPlatformType(type);

		if (state == ResponseState.Success) {
			Debug.Log ("get user info result :");
			Debug.Log (MiniJSON.jsonEncode(result));
			Debug.Log ("AuthInfo:" + MiniJSON.jsonEncode (_SSDK.GetAuthInfo (type)));
			Debug.Log ("Get userInfo success !Platform :" + type );
			
			string userInfo = MiniJSON.jsonEncode(result);
			string authInfo = MiniJSON.jsonEncode(_SSDK.GetAuthInfo (type));

			if (_AuthorLuaFunc != null) {
				_AuthorLuaFunc.Call((int)ThirdResponseState.Sucess, "授权成功", platformType, userInfo,authInfo);
			}
		}
		else if (state == ResponseState.Fail) {
			#if UNITY_ANDROID
			Debug.Log ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
			#endif

			if (_AuthorLuaFunc != null) {
				_AuthorLuaFunc.Call((int)ThirdResponseState.Fail, "获取用户信息失败", platformType);
			}
		}
		else if (state == ResponseState.Cancel) {
			Debug.Log ("cancel !");

			if (_AuthorLuaFunc != null) {
				_AuthorLuaFunc.Call((int)ThirdResponseState.Cancel, "取消获取用户信息", platformType);
			}
		}
	}
	
	private static void OnShareResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result) {

		int platformType = ThirdPlatformTool.fromPlatformType(type);

		if (state == ResponseState.Success) {
			Debug.Log ("share successfully - share result :");
			Debug.Log (MiniJSON.jsonEncode(result));

			if (_ShareLuaFunc != null) {
				_ShareLuaFunc.Call((int)ThirdResponseState.Sucess, "分享成功", platformType);
			}

		}
		else if (state == ResponseState.Fail) {
			#if UNITY_ANDROID
			Debug.Log ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			_ShareLuaFunc.Call((int)ThirdResponseState.Fail, result["msg"], platformType);

			#elif UNITY_IPHONE
			Debug.Log ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
			_ShareLuaFunc.Call((int)ThirdResponseState.Fail, result["error_msg"], platformType);
			#endif
		}
		else if (state == ResponseState.Cancel) {
			Debug.Log ("cancel !");

			if (_ShareLuaFunc != null) {
				_ShareLuaFunc.Call((int)ThirdResponseState.Cancel, "分享取消", platformType);
			}
		}
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
		else if (thirdPlatformType == ThirdPlatformType.WeChatMoments) {
			finaltype = PlatformType.WeChatMoments;
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
		else if (type == PlatformType.WeChatMoments) {
			finaltype = (int)ThirdPlatformType.WeChatMoments;
		}

		return finaltype;
	}

}
