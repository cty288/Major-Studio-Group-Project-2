using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.EventSystems;

public class AlienBody : AbstractMikroController<MainGame>, IPointerClickHandler {

    public BodyInfo BodyInfo;

    private Transform tallSpawnPosition;
    private Transform shortSpawnPosition;

    private List<SpriteRenderer> spriteRenderers;
    

    private void Awake() {
        tallSpawnPosition = transform.Find("TallSpawnPosition");
        shortSpawnPosition = transform.Find("LowSpawnPosition");
       
    }

    public void OnBuilt() {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    public void Hide() {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
            spriteRenderer.DOFade(0, 0.5f);
        }
    }

    public void Show() {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
            spriteRenderer.DOFade(1, 0.5f);
        }
    }

    public static GameObject BuildShadowAlienBody(BodyInfo info, bool hide) {
        ResLoader resLoader = MainGame.Interface.GetUtility<ResLoader>();
        GameObject body = resLoader.LoadSync<GameObject>("aliens","AlienBody");
        GameObject bodyInstance = GameObject.Instantiate(body);
        AlienBodyPartInfo lastInfo = null;
        AlienBody alienBody = bodyInstance.GetComponent<AlienBody>();
        foreach (AlienBodyPartInfo partInfo in info.AllBodyInfoPrefabs) {
            Vector2 position = Vector2.zero;
            if (lastInfo != null) {
                position = lastInfo.JointPoint.position;
            }

            if (partInfo) {
                GameObject spawnedBodyPart = Instantiate(partInfo.gameObject, position, Quaternion.identity, bodyInstance.transform);
                lastInfo = spawnedBodyPart.GetComponent<AlienBodyPartInfo>();
                if (hide) {
                    spawnedBodyPart.GetComponentsInChildren<SpriteRenderer>(true).ToList()
                        .ForEach(s => s.color = new Color(1, 1, 1, 0));
                }
            }
        }

        alienBody.OnBuilt();
        return bodyInstance;
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

        int layer = 1;
        foreach (AlienBodyPartInfo partInfo in info.AllBodyInfoPrefabs) {
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

            if (partInfo)
            {
                GameObject spawnedBodyPart = Instantiate(partInfo.gameObject, position, Quaternion.identity, bodyInstance.transform);
                spawnedBodyPart.GetComponentInChildren<SpriteRenderer>().sortingOrder = layer;
                
               
                
                lastInfo = spawnedBodyPart.GetComponent<AlienBodyPartInfo>();
            }
            layer++;
        }
        alienBody.OnBuilt();
        return bodyInstance;
    }

    public Action onClickAlienBody;
    public void OnPointerClick(PointerEventData eventData) {
        onClickAlienBody?.Invoke();
    }
}
