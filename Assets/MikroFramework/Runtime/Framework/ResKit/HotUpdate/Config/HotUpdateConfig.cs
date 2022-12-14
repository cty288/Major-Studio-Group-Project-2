using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MikroFramework.ResKit;
using MikroFramework.Serializer;
using MikroFramework.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace MikroFramework.ResKit
{
    public static class HotUpdateConfig {
        
        /// <summary>
        /// Path that hot update asset bundles are saved
        /// </summary>
        public static string HotUpdateAssetBundlesFolder {
            get { return PlayerPrefs.GetString("HotupdateFolder", Application.persistentDataPath + "/AssetBundles/"); }
        }

        /// <summary>
        /// Path that the local Res Version file saved
        /// </summary>
        public static string LocalResVersionFilePath {
            get {
#if UNITY_EDITOR
                return Application.streamingAssetsPath + "/AssetBundles/" + ResKitUtility.CurrentPlatformName +
                       "/ResVersion.json";
#else
                if (Application.platform == RuntimePlatform.Android) {
                  //  return Application.dataPath + "!assets"+ "/AssetBundles/" + ResKitUtility.CurrentPlatformName + "/ResVersion.json";
                 return Application.streamingAssetsPath + "/AssetBundles/" + ResKitUtility.CurrentPlatformName +
                                       "/ResVersion.json";
                }
                else {
                      return Application.streamingAssetsPath + "/AssetBundles/" + ResKitUtility.CurrentPlatformName +
                       "/ResVersion.json";
                }
#endif

            }
        }

        /// <summary>
        /// Path that the local AssetBundle files saved
        /// </summary>
        public static string LocalAssetBundleFolder {
            get {
#if UNITY_EDITOR
                return Application.streamingAssetsPath + "/AssetBundles/" + ResKitUtility.CurrentPlatformName + "/";
#else
                if (Application.platform == RuntimePlatform.Android) {
                    return Application.streamingAssetsPath + "/AssetBundles/" + ResKitUtility.CurrentPlatformName + "/";
                   // return Application.dataPath + "!assets"+ "/AssetBundles/" + ResKitUtility.CurrentPlatformName + "/";
                }
                else {
                    return Application.streamingAssetsPath + "/AssetBundles/" + ResKitUtility.CurrentPlatformName + "/";
                }
#endif

            }
        }

        /// <summary>
        /// URL for ResVersion file that saved on the remote server
        /// </summary>
        public static string RemoteResVersionURL {
            get {
                
                return "https://mikrocosmos2.sfo3.digitaloceanspaces.com/AssetBundles/"  +
                       ResKitUtility.CurrentPlatformName + "/ResVersion.json";
            }
        }

        /// <summary>
        /// URL in which Asset Bundles are saved on the remote server
        /// </summary>
        public static string RemoteAssetBundleBaseURL {
            get {
                return @"https://mikrocosmos2.sfo3.digitaloceanspaces.com/AssetBundles" + "/" + ResKitUtility.CurrentPlatformName + "/";
            }
        }

#if UNITY_EDITOR
        public static string AssetBundleAssetDataBuildPath
        {
            get {
                return UnityEditor.EditorPrefs.GetString("ABDataPath", Application.dataPath + "/AssetBundleBuilds/")
                    +"AssetBundles/"+ResKitUtility.CurrentPlatformName+"/";
            }
        }
#endif



        /// <summary>
        /// Get HotUpdate ResVersion from HotUpdate folder
        /// </summary>
        /// <returns></returns>
        public static ResVersion LoadHotUpdateAssetBundlesFolderResVersion() {
            string hotUpdateResVersionFilePath = HotUpdateAssetBundlesFolder + "ResVersion.json";


            if (!File.Exists(hotUpdateResVersionFilePath)) {
                return null;
            }

            string persistResVersionJson = File.ReadAllText(hotUpdateResVersionFilePath);
            ResVersion persistResVersion = AdvancedJsonSerializer.Singleton.Deserialize<ResVersion>(persistResVersionJson);

            return persistResVersion;

        }

       


    }

    //add manifest name
        //remoteResVersion.AssetBundleNames.Add(ResKitUtility.CurrentPlatformName);
}

