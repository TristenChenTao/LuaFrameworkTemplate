using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using LuaFramework;

using LuaInterface;
using BestHTTP;
using BestHTTP.Cookies;
using LitJson;

public class HTTPClient {

	enum HTTPResponseState : int {
		Sucess = 1,
		Fail = 0
    };　

	public static string com_pt ="";

	public static string GetComPt(){
		#if UNITY_ANDROID
			
			 com_pt = "2";
		#elif UNITY_IPHONE
			
			  com_pt = "1";
		#endif
		return com_pt;
	}

	public static bool IsEditor(){
		bool isEditor = false;
		#if UNITY_EDITOR
			
	
			isEditor = true;
		#endif
		return isEditor;
	}
	public static string MaJiang_UID;
	public static string MaJiang_UID_MD5;
	public static string MaJiang_NickName;

	public static void Request (int methodType, string url, WWWForm parameter,  LuaFunction func = null) {
		
		HTTPMethods type = HTTPMethods.Get;

		if(methodType == 1) {
			type = HTTPMethods.Get;
		}
		else if (methodType == 2) {
			type = HTTPMethods.Post;
		}

		Debug.Log("开始请求 Url： " + url);
		Debug.Log("parameter： " + parameter.ToString());

		HTTPRequest httpRequest = new HTTPRequest(new Uri(url),
         type,
        (req, response) => {
			int code = 0;
			string message = "";
			string dataString = "";

            if (req.Exception != null){

				message = req.Exception.ToString();
				if (func != null) {
                    func.Call((int)HTTPResponseState.Fail, code, message, dataString);
                }
            }
            else {
                try {
                    if (response!=null) {
                        Debug.Log("请求的Url: "+url+"=====>"+response.DataAsText);
                    	JsonData jsonData = JsonMapper.ToObject(response.DataAsText);

                    	code = int.Parse(jsonData["ResultCode"].ToString());
						message = jsonData["ResultMessage"].ToString();

						JsonData data = jsonData["Data"];
						if(data != null) {
							dataString = JsonMapper.ToJson(data);
						}
                    }

					Debug.Log("请求成功 Url： " + url);
					Debug.Log("code： " + code);
					Debug.Log("message " + message);
					Debug.Log("dataString " + dataString);

					if (func != null) {
                    	func.Call((int)HTTPResponseState.Sucess, code, message, dataString);
                	}
                }
                catch (Exception ex) {
                    Debug.Log(ex);
					message = ex.ToString();
					if (func != null) {
                    	func.Call((int)HTTPResponseState.Fail, code, message, dataString);
                	}
                }
            }
        });
		
		if(parameter == null) {
			parameter = new WWWForm();
		}
		
		parameter.AddField("Com_Ver", AppConst.Product_Version);
		
		if(!string.IsNullOrEmpty(MaJiang_UID)) {
			parameter.AddField("MaJiang_UID", MaJiang_UID);
		}
		if(!string.IsNullOrEmpty(MaJiang_NickName)) {
			parameter.AddField("MaJiang_NickName", MaJiang_NickName);
		}
		if(!string.IsNullOrEmpty(MaJiang_UID_MD5)) {
			parameter.AddField("MaJiang_UID_MD5", MaJiang_UID_MD5);
				Debug.Log("MaJiang_UID_MD5.Uri" );
		}else{
			Debug.Log("MaJiang_UID_MD5.===null" );
		}
		// parameter.AddField("MaJiang_NickName", "ceshi2");
		// 	parameter.AddField("MaJiang_UID_MD5", "E02DEB3D27D57FF0143306A4972CE9AB");
		// 	parameter.AddField("MaJiang_UID", 6);
		#if UNITY_ANDROID
			 parameter.AddField("com_pt", 2);
			
		#elif UNITY_IPHONE
			 parameter.AddField("com_pt", 1);
			 
		#endif

        httpRequest.SetFields(parameter);
	
		
		//BestHTTP.Cookies.CookieJar.Clear();  
		
        httpRequest.Send();
	}

	public static void LoadWebImage (string url, LuaFunction func = null) {

		string message = "";
		Texture2D image = null;

		if (string.IsNullOrEmpty(url)) {
			func.Call((int)HTTPResponseState.Fail, "URL图片地址为空",image);
			return;
        }

		HTTPRequest request = new HTTPRequest(new Uri(url), (req, response) => {
			if (req.Exception != null) {
				message = "请求图片地址 "+ url + req.Exception.ToString();
				if (func != null) {
                    func.Call((int)HTTPResponseState.Fail, message, image);
                }
            }
			else {
				if (response != null) {
					image = response.DataAsTexture2D;
					message = "请求图片地址 "+ url + "成功";
					func.Call((int)HTTPResponseState.Sucess, message, image);
            	}
				else {
					message = "请求图片地址 "+ url + "内容不存在";
					func.Call((int)HTTPResponseState.Fail, "URL图片地址为空", image);
				}
			}

        });
        request.Send();
	}
}
