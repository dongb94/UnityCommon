using Almond.Network;
using k514;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

/// <summary>
/// 유니티에서 제공하는 인앱 결제 기능을 구현한 클래스 (구글, 애플 공통)
/// </summary>
public class UnityInAppPurchasing : Singleton<UnityInAppPurchasing> , IStoreListener 
{
    private IStoreController controller;
    private IExtensionProvider extensions;

    public void OnClickPurchaseBtn(int index)
    {
        if (controller == null)
        {
            // 결제기능 초기화 실패

            return;
        }
        controller.InitiatePurchase("diamond_10");
    }
    

    /// <summary>
    /// 구매가 완료되면 호출되는 메소드
    /// 구매 내역 실행, 서버에 영수증 전송
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        bool validPurchase = true; // Presume valid for platforms with no R.V.

        // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

        try {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(e.purchasedProduct.receipt);
            // For informational purposes, we list the receipt(s)
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result) {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        } catch (IAPSecurityException) {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif

        if (validPurchase) {
            // 서버에 영수증 전송
        }

        // Complete = 즉시 완료 메세지를 Google에 보내고 거래를 완료한다.
        // Pending = ConfirmPendingPurchase 메소드를 통해 결제 완료를 알릴때 까지 대기한다.
        return PurchaseProcessingResult.Complete;
        
    }

    /// <summary>
    /// 구매 실패시 호출되는 메소드
    /// </summary>
    /// <param name="i"></param>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// IAP 초기화에 성공했을 때 호출됨
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="extensions"></param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
    }
    
    /// <summary>
    /// IAP 초기화에 실패했을 때 호출됨
    /// </summary>
    /// <param name="error"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }
    
    public override void OnCreated()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct("diamond_10", ProductType.Consumable, new IDs
        {
            {"diamond_10_google", "googleplay"},
            {"diamond_10_mac", "appstore"}
        });

        UnityPurchasing.Initialize (this, builder);
    }
}
