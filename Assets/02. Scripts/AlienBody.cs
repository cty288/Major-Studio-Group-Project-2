using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.ResKit;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public interface IHaveBodyInfo {
    public BodyInfo BodyInfo { get; set; }
}

public class AlienBody : AbstractMikroController<MainGame>, IPointerClickHandler, IHaveBodyInfo, ICanSendEvent {

    public BodyInfo BodyInfo { get; set; }

    private Transform tallSpawnPosition;
    private Transform shortSpawnPosition;

    private List<SpriteRenderer> spriteRenderers;

    public Action onClickAlienBody;

    private BountyHunterSystem bountyHunterSystem;

    protected Speaker speaker;
    protected PlayerResourceSystem playerResourceSystem;

    protected bool onClickPeepholeSpeakEnd = false;
    protected virtual void Awake() {
        tallSpawnPosition = transform.Find("TallSpawnPosition");
        shortSpawnPosition = transform.Find("LowSpawnPosition");
        bountyHunterSystem = this.GetSystem<BountyHunterSystem>();
        speaker = GameObject.Find("OutsideBodySpeaker").GetComponent<Speaker>();
        playerResourceSystem = this.GetSystem<PlayerResourceSystem>();
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


    public static GameObject BuildShadowBody(BodyInfo info, bool hide, string prefabName) {
        ResLoader resLoader = MainGame.Interface.GetUtility<ResLoader>();
        GameObject body = resLoader.LoadSync<GameObject>("aliens", prefabName);
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

        alienBody.BodyInfo = info;
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
        alienBody.BodyInfo = info;
        alienBody.OnBuilt();
        return bodyInstance;
    }
    public void OnPointerClick(PointerEventData eventData) {
        if (bountyHunterSystem.IsBountyHunting) {
            return;
        }
        onClickAlienBody?.Invoke();
    }

    public virtual Func<bool> OnBodyClickedOnPeephole() {
        onClickPeepholeSpeakEnd = false;
        if (BodyInfo.IsAlien) {
            speaker.Speak("Hahaha! I will kill you!", null, OnAlienClickedOutside);
        }
        else {
            speaker.Speak("Hey, I brought you some foods! Take care!", null, OnDelivererClickedOutside);
        }
        return () => onClickPeepholeSpeakEnd;
    }

    private void OnDelivererClickedOutside() {
        this.GetSystem<PlayerResourceSystem>().AddFood(Random.Range(1, 3));
        this.SendEvent<OnShowFood>();
        this.Delay(4.5f, () => {
            onClickPeepholeSpeakEnd = true;
        });
    }

    private void OnAlienClickedOutside() {
        if (playerResourceSystem.HasEnoughResource<BulletGoods>(1) && playerResourceSystem.HasEnoughResource<GunResource>(1)) {
            playerResourceSystem.RemoveResource<BulletGoods>(1);
            float clipLength = AudioSystem.Singleton.Play2DSound("gun_fire").clip.length;
            
            this.Delay(1f, () => {
                LoadCanvas.Singleton.ShowMessage("You shot the creature and it fleed.\n\nBullet - 1");
                this.Delay(2f, () => {
                    LoadCanvas.Singleton.HideMessage();
                    this.Delay(1f, () => {
                        onClickPeepholeSpeakEnd = true;
                    });
                });
            });
        }
        else {
            DieCanvas.Singleton.Show("You are killed by the creature!");
            this.GetModel<GameStateModel>().GameState.Value = GameState.End;
            this.GetSystem<BodyGenerationSystem>().StopCurrentBody();
        }
    }
}
