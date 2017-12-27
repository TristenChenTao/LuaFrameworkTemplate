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
			int state = 0; // 0 失败 1 成功
			int code = 0;
			string message = "";
			string dataString = "";

            if (req.Exception != null){

				message = req.Exception.ToString();
				if (func != null) {
                    func.Call(0,code, message, dataString);
                }
            }
            else {
                try {
                    if (response!=null) {
                        
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
                    	func.Call(0,code, message, dataString);
                	}
                }
                catch (Exception ex) {
                    Debug.Log(ex);
					message = ex.ToString();
					if (func != null) {
                    	func.Call(-1,code, message, dataString);
                	}
                }
            }
        });
		
		if(parameter == null) {
			parameter = new WWWForm();
		}
		
		parameter.AddField("Com_Ver", AppConst.Product_Version);
		
		#if UNITY_ANDROID
			 parameter.AddField("Com_GamePt", 2);
		#elif UNITY_IPHONE
			 parameter.AddField("Com_GamePt", 1);
		#endif
       
        httpRequest.SetFields(parameter);
        httpRequest.Send();
	}
}
