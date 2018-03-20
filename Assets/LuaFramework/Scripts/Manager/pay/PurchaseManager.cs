using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using LitJson;
using LuaInterface;
using UnityEngine;
using UnityEngine.Purchasing;
public class PurchaseKey {

    public static string DidNotAllowPurchaseKey = "didNotAllowPurchase"; //没有权限
    public static string didAllProductKey = "didAllProduc"; //所有产品 @param UnityEngine.Purchasing.Product
    public static string didProductPurchaseSuccessKey = "didProductPurchaseSuccess"; //成功 

    public static string didProductPurchaseFailureKey = "didProductPurchaseFailure"; //失败 @param string
}

public class PurchaseManager : MonoBehaviour, IStoreListener {
    static string saveReceiptKey = "SaveReceiptKey";

    public static List<string> ProductID = new List<string> ();

    private IStoreController controller;

    public static string URL_Domain = "";

    public static string iosPurchaseRecharge = "";

    int reconnectCount = 0;
    int ReconnectMax = 3;
    PayInfoModel infoModel;
    LuaFunction mSuccess;
    LuaFunction mfailure;
    LuaFunction IosPurchList;

    public PurchaseManager () {

    }

    public void initIOSPlay (LuaFunction iosPurchList) {
        this.IosPurchList = iosPurchList;
        var module = StandardPurchasingModule.Instance ();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance (module);
        foreach (string element in ProductID){
            builder.AddProduct (element, ProductType.Consumable);
        }

        UnityPurchasing.Initialize (this, builder);
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized (IStoreController controller, IExtensionProvider extensions) {

        this.controller = controller;
        IosPurchList.Call (MiniJSON.jsonEncode (controller.products.all));
        foreach (Product product in controller.products.all) {
            Debug.Log ("OnInitialized localizedPriceString" + product.metadata.localizedPriceString);
            Debug.Log ("OnInitialized localizedPrice" + product.metadata.localizedPrice);
            Debug.Log ("OnInitialized localizedDescription" + product.metadata.localizedDescription);
            Debug.Log ("OnInitialized isoCurrencyCode" + product.metadata.isoCurrencyCode);
            Debug.Log ("OnInitialized localizedTitle" + product.metadata.localizedTitle);
        }

    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed (InitializationFailureReason error) {
        if (error == InitializationFailureReason.PurchasingUnavailable) {
            mfailure.Call ("手机设置了禁止APP内购");
            Debug.Log ("手机设置了禁止APP内购");
        }
        Debug.Log ("IAP初始化失败");

    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e) {
        try {
            JsonData jsonData = JsonMapper.ToObject (e.purchasedProduct.receipt);
            string receiptString = jsonData["Payload"].ToString ();

            saveReceiptString (receiptString);
            validateReceipt (receiptString);

            return PurchaseProcessingResult.Complete;
        } catch (Exception exception) {
            Debug.Log (exception);
            return PurchaseProcessingResult.Complete;
        }
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed (Product item, PurchaseFailureReason r) {
        mfailure.Call ("支付失败");
    }

    public void OnPurchaseClicked (string productId, PayInfoModel infoModel, LuaFunction Success,
        LuaFunction failure) {
        this.infoModel = infoModel;
        this.mSuccess = Success;
        this.mfailure = failure;
        controller.InitiatePurchase (productId);
    }

    void validateReceipt (string receiptString) {
        WWWForm param = new WWWForm ();
        param.AddField ("token", infoModel.token);
        param.AddField ("receiptData", receiptString);
        Debug.Log ("receiptData 111111 = " + receiptString);
        StartCoroutine (SendPostIos (URL_Domain + iosPurchaseRecharge, param));
    }
    IEnumerator SendPostIos (string _url, WWWForm _wForm) {
        WWW postData = new WWW (_url, _wForm);
        yield return postData;
        if (postData.error != null) {
            Debug.Log ("Unity 网络请求失败:" + postData.error);
            mfailure.Call ("验证失败");
        } else {
            mSuccess.Call ("支付成功");
            deleteReceiptString ();
            reconnectCount = 0;
        }
    }

    public void reValidateReceipt () {
        if (string.IsNullOrEmpty (loadReceiptString ())) {
            validateReceipt (loadReceiptString ());
        }
    }

    void reValidateReceiptAfterServerCheckFail () {
        if (reconnectCount < ReconnectMax) {
            reValidateReceipt ();
            reconnectCount += 1;
        }
    }
    void saveReceiptString (string recepit) {
        if (string.IsNullOrEmpty (recepit)) {
            PlayerPrefs.SetString (saveReceiptKey, recepit);
        }
    }
    string loadReceiptString () {
        if (PlayerPrefs.HasKey (saveReceiptKey)) {
            return PlayerPrefs.GetString (saveReceiptKey);
        }

        return "";
    }

    void deleteReceiptString () {
        PlayerPrefs.DeleteKey (saveReceiptKey);
    }

}