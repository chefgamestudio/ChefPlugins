#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using UnityEngine.iOS;
#endif

using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Facebook.Unity;
using GameAnalyticsSDK;

namespace gs.chef.plugins.attauth
{
    public class CHEF_ATT_AUTH_Manager
    {
        Version currentVersion;
        Version iOS_14_5;

        public async UniTask Initialize(CancellationToken token)
        {
            GameAnalytics.Initialize();
#if UNITY_IOS && !UNITY_EDITOR
            currentVersion = new Version(Device.systemVersion);
            iOS_14_5 = new Version("14.5");
#endif

#if UNITY_IOS && !UNITY_EDITOR
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            Debug.Log(
                $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: Start] Status : {status} | CurrentVersion : {currentVersion} | Compare Version (is higher 14.5) : {currentVersion >= iOS_14_5}");

            if (currentVersion >= iOS_14_5)
            {
                
                
                if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    ATTrackingStatusBinding.RequestAuthorizationTracking(AuthorizationTrackingReceived);

                    if (!token.IsCancellationRequested)
                    {
                        while (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                        {
                            status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
                            await UniTask.Delay(100);
                        }    
                    }
                    else
                    {
                        Debug.Log("- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] ATTAuthManager Initialize Cancelled");
                    }
                }
                else
                {
                    InitializeFB();
                }
            }
            else
            {
                InitializeFB();
            }

            //if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED && currentVersion >= iOS_14_5)
            //{
            //    ATTrackingStatusBinding.RequestAuthorizationTracking(AuthorizationTrackingReceived);
            //}
#else
            Debug.Log(
                $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: Start] | MESSAGE : The platform is not iOS. | InitializeFB called...");
            InitializeFB();
#endif

            await UniTask.CompletedTask;
        }

        private void AuthorizationTrackingReceived(int status)
        {
            Debug.Log(
                $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: AuthorizationTrackingReceived] | Authorization Tracking Status : {status}");
            InitializeFB();
        }

        private void InitializeFB()
        {
            Debug.Log(
                $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: InitializeFB] | FB.IsInitialized : {FB.IsInitialized}");

            if (!FB.IsInitialized)
            {
                Debug.Log(
                    $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: InitializeFB] | FB.Init called...");
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                Debug.Log(
                    $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: InitializeFB] | ActivateFB called...");
                //FB.ActivateApp();
                ActivateFB();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitCallback()
        {
            Debug.Log(
                $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: InitCallback] | FB.IsInitialized : {FB.IsInitialized}");
            if (FB.IsInitialized)
            {
                Debug.Log(
                    $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: InitCallback] | ActivateFB called...");
                ActivateFB();
            }
            else
            {
                Debug.Log(
                    $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: InitCallback] | ERROR : Failed to Initialize the Facebook SDK");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ActivateFB()
        {
#if UNITY_IOS && !UNITY_EDITOR
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

        if (currentVersion >= iOS_14_5)
        {
            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
            {
                Debug.Log($"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: ActivateFB] | FB.Mobile.SetAdvertiserTrackingEnabled(true) called...");
                FB.Mobile.SetAdvertiserTrackingEnabled(true);
            }
            else
            {
                Debug.Log($"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: ActivateFB] | FB.Mobile.SetAdvertiserTrackingEnabled(false) called...");
                FB.Mobile.SetAdvertiserTrackingEnabled(false);
            }
        }
        else
        {
            Debug.Log($"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: ActivateFB] | FB.Mobile.SetAdvertiserTrackingEnabled(false) called...");
            FB.Mobile.SetAdvertiserTrackingEnabled(false);
        }

#endif
            Debug.Log(
                $"- - - - - - - - - - - - - - -\n[Class: CHEF_ATTAuth_Manager] [Method: ActivateFB] | FB.ActivateApp called...");
            FB.ActivateApp();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isGameShown"></param>
        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}