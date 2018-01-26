using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PayInfoModel
{

    public string payType;
    public string osType;
    public string token;
    public string productId;

}

public enum PayType
{
    Wechat = 1,
    AliPay = 2
}

public enum PayTypePlatformType
{
    ios = 1,
    android = 2
}
