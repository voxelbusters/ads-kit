using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary.NativePlugins.DemoKit;

namespace VoxelBusters.AdsKit.Demo
{
	public enum AdsKitDemoActionType
	{
		RegisterListener = 1,
		UnregisterListener,
		ConsentFormProviderAvailable,
		Initialise,
		LoadAd,
        ShowAd,
        HideAd,
		AdPlacementState
	}

	public class AdsKitDemoAction : DemoActionBehaviour<AdsKitDemoActionType> 
	{ }
}