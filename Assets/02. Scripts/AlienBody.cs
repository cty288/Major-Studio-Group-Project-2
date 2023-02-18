using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Extensions;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public interface IHaveBodyInfo {
    public List<BodyInfo> BodyInfos { get; set; }
}

public class AlienBody : AbstractMikroController<MainGame>, IPointerClickHandler, IHaveBodyInfo, ICanSendEvent {

    public List<BodyInfo> BodyInfos { get; set; } = new List<BodyInfo>();

    private Transform tallSpawnPosition;
    private Transform shortSpawnPosition;

    private List<SpriteRenderer> spriteRenderers;

    public Action onClickAlienBody;

    private BountyHunterSystem bountyHunterSystem;

    
    private List<AlienBodyPartInfo> alienBodyPartInfos = new List<AlienBodyPartInfo>();

    protected virtual void Awake() {
        tallSpawnPosition = transform.Find("TallSpawnPosition");
        shortSpawnPosition = transform.Find("LowSpawnPosition");
        bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        
    }

    public void OnBuilt() {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    public void Hide() {
        foreach (AlienBodyPartInfo info in alienBodyPartInfos) {
            info.Disappear(0.5f);
        }
        
    }

    public void Show() {
        foreach (AlienBodyPartInfo info in alienBodyPartInfos) {
            info.Appear(0.5f);
        }
    }

    public void ShowColor(float fadeTime) {
        foreach (AlienBodyPartInfo info in alienBodyPartInfos) {
            info.ShowColor(fadeTime, Color.white);
        }
    }

    public void HideColor(float fadeTime) {
        foreach (AlienBodyPartInfo info in alienBodyPartInfos) {
            info.HideColor(fadeTime);
        }
    }


    public static GameObject BuildShadowBody(BodyInfo info, bool hide, string prefabName) {
        ResLoader resLoader = MainGame.Interface.GetUtility<ResLoader>();
        GameObject body = resLoader.LoadSync<GameObject>("aliens", prefabName);
        GameObject bodyInstance = GameObject.Instantiate(body);
        AlienBodyPartInfo lastInfo = null;
        AlienBody alienBody = bodyInstance.GetComponent<AlienBody>();
        foreach (BodyPartPrefabInfo partInfo in info.AllBodyInfoPrefabs) {
            if (partInfo == null) {
                continue;
            }
            Vector2 position = Vector2.zero;
            if (lastInfo != null) {
                position = lastInfo.JointPoint.position;
            }

            if (partInfo.BodyPartInfo) {
                GameObject spawnedBodyPart = Instantiate(partInfo.BodyPartInfo.gameObject, position, Quaternion.identity, bodyInstance.transform);
                lastInfo = spawnedBodyPart.GetComponent<AlienBodyPartInfo>();
                lastInfo.Init(partInfo);
                
                
            }
            
        }
        alienBody.alienBodyPartInfos = bodyInstance.GetComponentsInChildren<AlienBodyPartInfo>(true).ToList();
        if (hide) {
            foreach (AlienBodyPartInfo bodyInfo in alienBody.alienBodyPartInfos) {
                bodyInfo.HideInstantly();
            }
        }
        alienBody.BodyInfos = new List<BodyInfo>() {info};
       
        alienBody.OnBuilt();
        return bodyInstance;
    }
    public static GameObject BuildShadowBody(BodyInfo info, bool hide) {
        return BuildShadowBody(info, hide, "AlienBody");
    }
    public static GameObject BuildNewspaperAlienBody(BodyInfo info, int index) {
        ResLoader resLoader = MainGame.Interface.GetUtility<ResLoader>();
        GameObject body = resLoader.LoadSync<GameObject>("aliens", $"NewspaperFrame_{index}");
        GameObject bodyInstance = GameObject.Instantiate(body);
        AlienBody alienBody = bodyInstance.GetComponent<AlienBody>();

        Camera renderCamera = bodyInstance.transform.Find("RenderCamera").GetComponent<Camera>();
        RenderTexture renderTexture = new RenderTexture(renderCamera.targetTexture);
        renderCamera.targetTexture = renderTexture;

        bodyInstance.transform.position = new Vector3(500, index * 100, 0);

        AlienBodyPartInfo lastInfo = null;

       // int layer = 1;
        foreach (BodyPartPrefabInfo partInfo in info.AllBodyInfoPrefabs) {
            if (partInfo == null) {
                continue;
            }
            Vector2 position = Vector2.zero;
            if (lastInfo != null) {
                position = lastInfo.JointPoint.position;
            }else {
                if (info.Height == HeightType.Short) {
                    position = alienBody.shortSpawnPosition.position;
                }else {
                    position = alienBody.tallSpawnPosition.position;
                }
            }

            if (partInfo.BodyPartInfo) {
                GameObject spawnedBodyPart = Instantiate(partInfo.BodyPartInfo.gameObject, position, Quaternion.identity, bodyInstance.transform);
                //spawnedBodyPart.GetComponentInChildren<SpriteRenderer>().sortingOrder = layer;
                spawnedBodyPart.GetComponent<AlienBodyPartInfo>().Init(partInfo);
                
                
                lastInfo = spawnedBodyPart.GetComponent<AlienBodyPartInfo>();
            }
            //layer++;
        }
        alienBody.BodyInfos = new List<BodyInfo>(){info};
        alienBody.alienBodyPartInfos = bodyInstance.GetComponentsInChildren<AlienBodyPartInfo>(true).ToList();
        alienBody.OnBuilt();
        return bodyInstance;
    }
    public void OnPointerClick(PointerEventData eventData) {
        if (bountyHunterSystem.IsBountyHunting) {
            return;
        }
        onClickAlienBody?.Invoke();
    }


    
}
