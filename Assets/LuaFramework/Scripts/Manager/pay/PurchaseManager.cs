using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using BestHTTP;
using System;
using LitJson;

public class PurchaseKey
{

    public static string DidNotAllowPurchaseKey = "didNotAllowPurchase";//没有权限
    public static string didAllProductKey = "didAllProduc";//所有产品 @param UnityEngine.Purchasing.Product
    public static string didProductPurchaseSuccessKey = "didProductPurchaseSuccess";//成功 

    public static string didProductPurchaseFailureKey = "didProductPurchaseFailure"; //失败 @param string
}


public class PurchaseManager :MonoBehaviour, IStoreListener
{
    static string saveReceiptKey = "SaveReceiptKey";

    public static string[] ProductID = { "YYM1", "YYM6", "YYM30", "YYM68", "YYM128", "YYM648" };

    private IStoreController controller;

    int reconnectCount = 0;
    int ReconnectMax = 3;
    PayInfoModel infoModel;
    Action<string> mSuccess;
    Action<string> mfailure;
    Action<UnityEngine.Purchasing.Product[]> IosPurchList;
    private static PurchaseManager instance;

    // Use this for initialization
    public static PurchaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PurchaseManager();
            }
            return instance;
        }
    }

    public PurchaseManager()
    {

    }

    public void initIOSPlay(Action<UnityEngine.Purchasing.Product[]> iosPurchList)
    {
        this.IosPurchList = iosPurchList;
        var module = StandardPurchasingModule.Instance();
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
        for (int i = 0; i < ProductID.Length; i++)
        {
            builder.AddProduct(ProductID[i], ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {

        this.controller = controller;
        IosPurchList(controller.products.all);
        foreach (Product product in controller.products.all)
        {
            Debug.Log("OnInitialized localizedPriceString" + product.metadata.localizedPriceString);
            Debug.Log("OnInitialized localizedPrice" + product.metadata.localizedPrice);
            Debug.Log("OnInitialized localizedDescription" + product.metadata.localizedDescription);
            Debug.Log("OnInitialized isoCurrencyCode" + product.metadata.isoCurrencyCode);
            Debug.Log("OnInitialized localizedTitle" + product.metadata.localizedTitle);
        }

    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        if (error == InitializationFailureReason.PurchasingUnavailable)
        {
            mfailure("手机设置了禁止APP内购");
            Debug.Log("手机设置了禁止APP内购");
        }
        Debug.Log("IAP初始化失败");

    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        try
        {
            JsonData jsonData = JsonMapper.ToObject(e.purchasedProduct.receipt);
            string receiptString = jsonData["Payload"].ToString();

            saveReceiptString(receiptString);
            validateReceipt(receiptString);

            return PurchaseProcessingResult.Complete;
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            return PurchaseProcessingResult.Complete;
        }
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
    {
        mfailure("支付失败");
    }

    public void OnPurchaseClicked(string productId, PayInfoModel infoModel, Action<string> Success,
                    Action<string> failure)
    {
        this.infoModel = infoModel;
        this.mSuccess = Success;
        this.mfailure = failure;
        controller.InitiatePurchase(productId);
    }

    void validateReceipt(string receiptString)
    {
            WWWForm param = new WWWForm();
            param.AddField("token", infoModel.token);
            param.AddField("receiptData", receiptString);
            Debug.Log("receiptData 111111 = " + receiptString);
            StartCoroutine(SendPostIos("http://test.wolf.esgame.com" + "/Account/VerifyReceipt", param));
    }
    IEnumerator SendPostIos(string _url, WWWForm _wForm)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (postData.error != null)
        {
            Debug.Log("Unity 网络请求失败:" + postData.error);
            mfailure( "验证失败");
        }
        else
        {
            mSuccess("支付成功");
            deleteReceiptString();
            reconnectCount = 0;
        }
    }

    public void reValidateReceipt()
    {
        if (string.IsNullOrEmpty(loadReceiptString()))
        {
            validateReceipt(loadReceiptString());
        }
    }

    void reValidateReceiptAfterServerCheckFail()
    {
        if (reconnectCount < ReconnectMax)
        {
            reValidateReceipt();
            reconnectCount += 1;
        }
    }
    void saveReceiptString(string recepit)
    {
        if (string.IsNullOrEmpty(recepit))
        {
            PlayerPrefs.SetString(saveReceiptKey, recepit);
        }
    }
    string loadReceiptString()
    {
        if (PlayerPrefs.HasKey(saveReceiptKey))
        {
            return PlayerPrefs.GetString(saveReceiptKey);
        }

        return "";
    }

    void deleteReceiptString()
    {
        PlayerPrefs.DeleteKey(saveReceiptKey);
    }


    
}