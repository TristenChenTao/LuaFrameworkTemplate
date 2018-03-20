using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;
using UnityEngine; //包含必要的库
using LuaInterface;

public class PayManager : MonoBehaviour {
    //android 微信支付
    public static PayManager instance = null;

    public static PayManager GetInstence () {
        return instance;
    }

    public static string URL_Domain = "";

    public static string WXRecharge = "";

    public static string ZfbRecharge = "";
    public static string iosPurchaseRecharge = "";

    public delegate void SocketDidDisconnect (string errorDes);
    LuaFunction mSuccess;
    LuaFunction mfailure;
    PurchaseManager iosPurcjase;

    public static List<string> ProductID = new List<string> ();
    public static void addIosPurpase (string productId) {
        ProductID.Add (productId);
    }
    public void initIos (LuaFunction IosPurchList) {
        //初始化ios
        if (iosPurcjase == null) {
            if (gameObject.GetComponent<PurchaseManager> ()) {
                iosPurcjase = gameObject.GetComponent<PurchaseManager> ();
            } else {
                iosPurcjase = gameObject.AddComponent<PurchaseManager> ();
            }
        }

        PurchaseManager.iosPurchaseRecharge = iosPurchaseRecharge;
        PurchaseManager.URL_Domain = URL_Domain;
        PurchaseManager.ProductID = ProductID;
        iosPurcjase.initIOSPlay (IosPurchList);
    }
    public void Pay (string productId,
        string token = "",
        string PayTypes = "",
        LuaFunction Success = null,
        LuaFunction failure = null) {
        PayInfoModel infoModel = new PayInfoModel ();
        infoModel.token = token;
#if UNITY_EDITOR  
        infoModel.osType = (int) PayTypePlatformType.ios + "";
        // Debug.Log("支付请在手机上运行");
        // return;
#elif UNITY_IPHONE  
        infoModel.osType = (int) PayTypePlatformType.ios + "";
#elif UNITY_ANDROID  
        infoModel.osType = (int) PayTypePlatformType.android + "";
#endif  

        infoModel.payType = PayTypes;

        infoModel.productId = productId;

        mSuccess = Success;
        mfailure = failure;
        if (infoModel == null) {
            Debug.Log ("付款信息对象不能为空");
            return;
        }
        Debug.Log ("C#中执行pay =" + infoModel.osType);
        if (infoModel.osType == (int) PayTypePlatformType.android + "") {
            //平台支付

            if (PayfromInt (infoModel.payType) == PayType.Wechat) {
                //微信支付
                //androind  微信支付
                WeChatPay (infoModel);

            } else if (PayfromInt (infoModel.payType) == PayType.AliPay) {
                //支付宝
                Alipay (infoModel);

            }
        } else {
            Debug.Log ("C#中执行pay2");
            //ios 内购
            iosPurcjase.OnPurchaseClicked (infoModel.productId, infoModel, mSuccess, mfailure);
        }

    }

    //微信支付
    private void WeChatPay (PayInfoModel infoModel) {
        WWWForm form = new WWWForm ();
        form.AddField ("token", infoModel.token);
        form.AddField ("productId", infoModel.productId);
        form.AddField ("osType", infoModel.osType);
        Debug.Log ("商品信息 请求发起 :");

        form.AddField ("Com_Ver", 1);
        form.AddField ("Com_GamePt", 1);

        StartCoroutine (SendPostWeChatPay (URL_Domain + WXRecharge, form));
    }

    IEnumerator SendPostWeChatPay (string _url, WWWForm _wForm) {
        WWW postData = new WWW (_url, _wForm);
        yield return postData;
        if (postData.error != null) {
            Debug.Log ("Unity 网络请求失败:" + postData.error);
        } else {
            Debug.Log ("Unity 网络请求成功:" + postData.text);
            string js = postData.text;
            int inx = js.IndexOf ("\"Data\"") + 7;
            int end = js.LastIndexOf ("}}");
            js = js.Substring (inx, end - inx + 1);
            Debug.Log (js);
            AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
            //调用对应方法
            jo.Call ("reqWXPay", js);
        }
    }
    IEnumerator SendPostALiPay (string _url, WWWForm _wForm) {
        WWW postData = new WWW (_url, _wForm);
        yield return postData;
        if (postData.error != null) {
            Debug.Log ("Unity 网络请求失败:" + postData.error);
        } else {
            Debug.Log ("Unity 网络请求成功:" + postData.text);
            string js = postData.text;
            int inx = js.IndexOf ("\"Data\"") + 8;
            int end = js.LastIndexOf ("\"}");
            js = js.Substring (inx, end - inx);
            js = DeUnicode (js);
            js = WWW.UnEscapeURL (js.ToString ());
            Debug.Log ("支付宝截取" + js);
            AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
            //调用对应方法
            jo.Call ("reqAliPay", js);
        }
    }

    //支付宝支付
    private void Alipay (PayInfoModel infoModel) {

        WWWForm form = new WWWForm ();
        form.AddField ("token", infoModel.token);
        form.AddField ("productId", infoModel.productId);
        form.AddField ("osType", infoModel.osType);
        form.AddField ("Com_Ver", 1);
        form.AddField ("Com_GamePt", 1);

        StartCoroutine (SendPostALiPay (URL_Domain + ZfbRecharge, form));

    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start () {
        if (instance != null) {
            Destroy (gameObject);
        } else {
            DontDestroyOnLoad (gameObject);
            instance = this;

        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update () {

    }

    string PayFail = "PayFail";
    string PaySucced = "PaySucced";
    string PayCancel = "PayCancel";
    string FailNoWX = "FailNoWX";
    public void PayState (string payState) {

        if (payState.Equals (PayFail)) {

            //  SendFailMessage("支付失败");
            mfailure.Call ("支付失败");
        } else if (payState.Equals (PaySucced)) {

            mSuccess.Call ("支付成功");

        } else if (payState.Equals (PayCancel)) {

            //   SendFailMessage("取消支付");
            mfailure.Call ("取消支付");
        } else if (payState.Equals (FailNoWX)) {

            // SendFailMessage("未安装微信");
            mfailure.Call ("未安装微信");
        }

    }

    public static string DeUnicode (string str) {
        //最直接的方法Regex.Unescape(str);
        Regex reg = new Regex (@"(?i)\\[uU]([0-9a-f]{4})");
        return reg.Replace (str, delegate (Match m) { return ((char) Convert.ToInt32 (m.Groups[1].Value, 16)).ToString (); });
    }

    private static PayTypePlatformType fromInt (string type) {

        PayTypePlatformType thirdPlatformType = (PayTypePlatformType) int.Parse (type);

        PayTypePlatformType finaltype = PayTypePlatformType.android;

        if (thirdPlatformType == PayTypePlatformType.android) {
            finaltype = PayTypePlatformType.android;
        } else if (thirdPlatformType == PayTypePlatformType.ios) {
            finaltype = PayTypePlatformType.android;
        }

        return finaltype;
    }
    private static PayType PayfromInt (string type) {

        PayType thirdPlatformType = (PayType) int.Parse (type);

        PayType finaltype = PayType.Wechat;

        if (thirdPlatformType == PayType.Wechat) {
            finaltype = PayType.Wechat;
        } else if (thirdPlatformType == PayType.AliPay) {
            finaltype = PayType.AliPay;
        }

        return finaltype;
    }
}