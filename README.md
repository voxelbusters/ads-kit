
# **Ads Kit for Unity** - Unified Open-Source Ad Integration Plugin

### **Monetize Your Unity Games with a Single API**

Ads Kit is an **open-source Unity plugin** that simplifies ad integration across multiple ad networks. With a **unified API**, you can easily manage **Banner, Interstitial, and Rewarded** ads, reducing the complexity of handling different SDKs.

🚀 **Currently Supported Ad Networks:**  
✅ **Google AdMob**  
✅ **AppLovin**  
✅ **Unity Level Play**

🔜 **Coming Soon:**  
🔹 **Facebook/Meta Audience Network**  
🔹 **LiftOff**

----------

## **✨ Features**

✅ **One Unified API** – Load and show ads from multiple networks using a single interface.  
✅ **Supports Banner, Interstitial, and Rewarded Ads** – A complete solution for your monetization needs.  
✅ **Mediation & Prioritization** – Optimize revenue by setting **custom ad network priorities**.  
✅ **Zero Extra Setup** – **Automatically configures** required details in `Info.plist` and `AndroidManifest.xml`.  
✅ **Auto-Loading Ads** – Ads can be preloaded in the background for seamless display.  
✅ **GDPR & Privacy Compliance** – Uses **Google’s UMP (User Messaging Platform)** for consent management.  
✅ **No-Code Solution** – Set up and manage ads **without writing any code**.

----------

## **📌 Getting Started**

### **1️⃣ Install Ads Kit**

Get latest unity package from [GitHub Releases](https://github.com/voxelbusters/ads-kit/releases/latest) and import it into your Unity project

or

Clone and merge contents of Assets folder with in your project.
```
git clone https://github.com/voxelbusters/ads-kit.git
```

### **2️⃣ Import Required Namespaces**

Before using Ads Kit, add the required namespaces:

```
using VoxelBusters.CoreLibrary; 
using VoxelBusters.AdsKit;
```

### **3️⃣ Initialize Ads Kit**

To start using Ads Kit, initialize it with a **consent form provider**:

```
public  void  InitialiseAdsManager(IConsentFormProvider consentProvider)
{
  var operation = AdsManager.Initialise(consentProvider);
  operation.OnComplete += (operationHandle) =>
    {
      // Ads Kit is now ready!
    };
}
``` 

#### **Get Consent Form Provider**

You can implement your own consent provider if you don't want the default consent provider (UPM from Google AdMob). Retrieve the available provider using:

```
private IConsentFormProvider GetConsentFormProvider()
{
    IConsentFormProvider availableProvider = AdServices.GetConsentFormProvider();

    if (availableProvider == null)
    {
      throw  new Exception("No IConsentFormProvider found. Implement IConsentFormProvider or enable AdMob for default consent form provider.");
    }

    return availableProvider;
}
``` 

----------

## **🎯 Using Ads Kit**

### **🔹 Load an Ad**

```
public  void  LoadAd(string placementId)
{
    AdsManager.LoadAd(placementId);
}
``` 

### **🔹 Show an Ad**

```
public  void  ShowAd(string placementId)
{
    AdsManager.ShowAd(placementId);
}
```

📌 **Ensure the ad is loaded before calling `ShowAd` if Auto Load is off.**

### **🔹 Hide an Ad (For Banners Only)**

```
public  void  HideAd(string placementId)
{
    AdsManager.HideAd(placementId);
}
```

📌 **Destroy the ad only if it’s no longer needed** (affects fill rate).

```
AdsManager.HideAd(placementId, destroy: true);
```

----------

## **🎛️ Registering Ad Events**

Track ad lifecycle events via **IAdLifeCycleListener** or attach callbacks to the `AsyncOperation` returned by API methods.

```
var operation = AdsManager.LoadAd("placementId");
operation.OnComplete += (operationHandle) =>
{   if (operationHandle.Error == null)
    {
        Debug.Log("Ad Loaded Successfully!");
    }
    else
    {
        Debug.LogError("Ad Failed to Load: " + operationHandle.Error);
    }
};
``` 

----------

## **📢 No-Code Solution**

If you prefer a **no-code implementation**, Ads Kit offers an easy way to integrate ads without writing a single line of code. Check the **No-Code Solution** section for more details.

----------

## **🛠️ Contributing to Ads Kit**

Ads Kit is a **community-driven project**, and we welcome all contributions!

💡 **Ways to contribute:**

-   Fork the repo and submit a pull request
    
-   Report bugs and suggest new features
    
-   Improve documentation
    

🎉 A huge thanks to all our contributors who make Ads Kit better every day!

🔗 **Start contributing today:** [GitHub Link](https://github.com/voxelbusters/ads-kit)

----------

## **📄 License**

Ads Kit is **open-source** and available under the **MIT License**.

📌 **MIT License – Free to use, modify, and distribute!**

----------

## **📥 Download & Get Started**

🚀 **[Download Ads Kit on GitHub](https://github.com/voxelbusters/ads-kit)** and start monetizing your Unity game today!
