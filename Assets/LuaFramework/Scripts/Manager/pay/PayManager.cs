using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;//包含必要的库

public class PayManager : MonoBehaviour
{
    //android 微信支付
    public static PayManager instance = null;

    public static PayManager GetInstence()
    {
        return instance;
    }

    public delegate void SocketDidDisconnect(string errorDes);
    Action<string> mSuccess;
    Action<string> mfailure;
    PurchaseManager iosPurcjase;
    public void initIos(Action<UnityEngine.Purchasing.Product[]> IosPurchList)
    {
        //初始化ios
        iosPurcjase = new PurchaseManager();
        iosPurcjase.initIOSPlay(IosPurchList);
    }
    public void Pay(string token,string Platform,string PayTypes,string id,
                    Action<string> Success,
                    Action<string> failure)
    {
        Hashtable table = new Hashtable();
        table["aa"] = "aa";
        PayInfoModel infoModel = new PayInfoModel();
        infoModel.token = token;
        infoModel.payType = PayTypes;
        infoModel.osType = Platform;
        infoModel.productId = id;
         Debug.Log("C#中执行pay");
        mSuccess = Success;
        mfailure = failure;
        if (infoModel == null)
        {
            Debug.Log("付款信息对象不能为空");
            return;
        }
        if (fromInt(infoModel.osType)== PayTypePlatformType.android)
        {
            //平台支付

            if (PayfromInt(infoModel.payType)== PayType.Wechat)
            {
                //微信支付
                //androind  微信支付
                WeChatPay(infoModel);

            }
            else if (PayfromInt(infoModel.payType) == PayType.AliPay)
            {
                //支付宝
                Alipay(infoModel);

            }
        }
        else
        {

            //ios 内购
            iosPurcjase.OnPurchaseClicked(infoModel.productId, infoModel, mSuccess, mfailure);
        }


    }


    //微信支付
    private void WeChatPay(PayInfoModel infoModel)
    {
        WWWForm form = new WWWForm();
        form.AddField("token", infoModel.token);
        form.AddField("productId", infoModel.productId);
        form.AddField("osType", infoModel.osType);
        Debug.Log("商品信息 请求发起 :");

        form.AddField("Com_Ver", 1);
        form.AddField("Com_GamePt", 1);

        StartCoroutine(SendPostWeChatPay( "http://test.wolf.esgame.com" + "/Account/WXRecharge", form));
    }

    IEnumerator SendPostWeChatPay(string _url, WWWForm _wForm)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log("Unity 网络请求失败:" + postData.error);
        }
        else
        {
            Debug.Log("Unity 网络请求成功:" + postData.text);
            string js = postData.text;
            int inx = js.IndexOf("\"Data\"") + 7;
            int end = js.LastIndexOf("}}");
            js = js.Substring(inx, end - inx + 1);
            Debug.Log(js);
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            //调用对应方法
            jo.Call("reqWXPay", js);
        }
    }
    IEnumerator SendPostALiPay(string _url, WWWForm _wForm)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log("Unity 网络请求失败:" + postData.error);
        }
        else
        {
            Debug.Log("Unity 网络请求成功:" + postData.text);
            string js = postData.text;
            int inx = js.IndexOf("\"Data\"") + 8;
            int end = js.LastIndexOf("\"}");
            js = js.Substring(inx, end - inx );
            js = DeUnicode(js);
            js =WWW.UnEscapeURL(js.ToString());
            Debug.Log("支付宝截取" + js);
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            //调用对应方法
            jo.Call("reqAliPay", js);
        }
    }




    //支付宝支付
    private void Alipay(PayInfoModel infoModel)
    {

        WWWForm form = new WWWForm();
        form.AddField("token", infoModel.token);
        form.AddField("productId", infoModel.productId);
        form.AddField("osType", infoModel.osType);
        form.AddField("Com_Ver", 1);
        form.AddField("Com_GamePt", 1);

        StartCoroutine(SendPostALiPay( "http://test.wolf.esgame.com" + "/Account/ZfbRecharge", form));

    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

    }

    string PayFail = "PayFail";
    string PaySucced = "PaySucced";
    string PayCancel = "PayCancel";
    string FailNoWX = "FailNoWX";
    public void PayState(string payState)
    {

        if (payState.Equals(PayFail))
        {

            //  SendFailMessage("支付失败");
            mfailure("支付失败");
        }
        else if (payState.Equals(PaySucced))
        {

            mSuccess("支付成功");

        }
        else if (payState.Equals(PayCancel))
        {

            //   SendFailMessage("取消支付");
            mfailure("取消支付");
        }
        else if (payState.Equals(FailNoWX))
        {

            // SendFailMessage("未安装微信");
            mfailure("未安装微信");
        }

    }


    
    public static string DeUnicode(string str)
    {
        //最直接的方法Regex.Unescape(str);
        Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        return reg.Replace(str, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
    }


    private static PayTypePlatformType fromInt(string type) {
		
		PayTypePlatformType thirdPlatformType = (PayTypePlatformType)int.Parse(type);

		PayTypePlatformType finaltype = PayTypePlatformType.android;

		if (thirdPlatformType == PayTypePlatformType.android) {
			finaltype = PayTypePlatformType.android;
		}
		else if (thirdPlatformType == PayTypePlatformType.ios) {
			finaltype = PayTypePlatformType.android;
		}

		return finaltype;
	}
    private static PayType PayfromInt(string type) {
		
		PayType thirdPlatformType = (PayType)int.Parse(type);

		PayType finaltype = PayType.Wechat;

		if (thirdPlatformType == PayType.Wechat) {
			finaltype = PayType.Wechat;
		}
		else if (thirdPlatformType == PayType.AliPay) {
			finaltype = PayType.AliPay;
		}

		return finaltype;
	}
}