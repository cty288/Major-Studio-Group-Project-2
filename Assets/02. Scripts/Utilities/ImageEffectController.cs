using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.ResKit;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Mikrocosmos
{
    public class ImageEffectController : MonoMikroSingleton<ImageEffectController>, IController {
        private ResLoader resLoader;
        private Renderer2DData render2DData;
        private void Awake() {
            resLoader = this.GetUtility<ResLoader>();
            render2DData = resLoader.LoadSync<Renderer2DData>("resources://MainRendererData");
         
        }

        public void DisableAllFeatures() {
            var features = render2DData.rendererFeatures;
            foreach (ScriptableRendererFeature feature in features)
            {
                feature.SetActive(false);
            }
        }
        private IEnumerator LoadRenderData() {
            while (resLoader==null || !resLoader.IsReady) {
                yield return null;
            }
           
        }

        public CustomRenderPassFeature GetScriptableRendererFeature(int index) {
            return render2DData.rendererFeatures[index] as CustomRenderPassFeature;
        }
        public Material GetScriptableRendererFeatureMaterial(int index) {
            CustomRenderPassFeature feature = render2DData.rendererFeatures[index] as CustomRenderPassFeature;
            return feature.settings.material;
        }
        public Material TurnOnScriptableRendererFeature(int index) {
            CustomRenderPassFeature feature = render2DData.rendererFeatures[index] as CustomRenderPassFeature;
            feature.SetActive(true);
            return feature.settings.material;
        }

        public void TurnOffScriptableRendererFeature(int index) {
            render2DData.rendererFeatures[index].SetActive(false);
        }

        
        protected override void OnApplicationQuit() {
            DisableAllFeatures();
            base.OnApplicationQuit();
        }

        
        public IArchitecture GetArchitecture() {
            return MainGame.Interface;
        }
    }
}
