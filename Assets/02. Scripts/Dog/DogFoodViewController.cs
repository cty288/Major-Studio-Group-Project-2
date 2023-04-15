using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.AlienInfos.Tags.Base.KnockBehavior;
using _02._Scripts.Dog;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogFoodViewController : AbstractMikroController<MainGame> {
    [SerializeField] 
    private Sprite dogDieSprite;
    private SpriteRenderer spriteRenderer;
    private TMP_Text infoText;

    private Coroutine dogEnvSoundCoroutine = null;
    
    private DogModel dogModel;
    private GameObject spawnedDog;
    [SerializeField] private Transform dogSpawnPosition;
    private void Awake() {
        this.RegisterEvent<OnDogGet>(OnDogGet).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnDogDie>(OnDogDie).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnKnockOutsideAudioPlayed>(OnKnockOutsideAudioPlayed)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnDogSendBack>(OnDogSendBack).UnRegisterWhenGameObjectDestroyed(gameObject);
        dogModel = this.GetModel<DogModel>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        infoText = transform.Find("Canvas/Text").GetComponent<TMP_Text>();
        gameObject.SetActive(false);

        if (!dogModel.isDogAlive) {
            spriteRenderer.sprite = dogDieSprite;
        }
    }

    private void OnDogSendBack(OnDogSendBack e) {
        if (dogEnvSoundCoroutine != null) {
            StopCoroutine(dogEnvSoundCoroutine);
            dogEnvSoundCoroutine = null;
        }
        gameObject.SetActive(false);
        DestroyDog();
    }

    private void OnKnockOutsideAudioPlayed(OnKnockOutsideAudioPlayed e) {
        if (dogModel.isDogAlive && dogModel.HaveDog && !dogModel.SentDogBack) {
            if (e.isAlien) {
                if (Random.Range(0f, 1f) < 0.2f) {
                    DogBark();
                }
            }
        }
    }

    private void DogBark() {
        AudioSystem.Singleton.Play2DSound($"dog_usual_{Random.Range(1, 3)}");
    }

    private void Start() {
        if (dogModel.isDogAlive && dogModel.HaveDog && !dogModel.SentDogBack) {
            dogEnvSoundCoroutine = StartCoroutine(DogEnvSound());
            SpawnDog(dogModel.MissingDogBodyInfo);
        }else if (!dogModel.isDogAlive) {
            infoText.text = "My friend is gone...";
        }
    }

    private void OnDogDie(OnDogDie e) {
        infoText.text = "My friend is gone...";
        spriteRenderer.sprite = dogDieSprite;
        if (dogEnvSoundCoroutine != null) {
            StopCoroutine(dogEnvSoundCoroutine);
            dogEnvSoundCoroutine = null;
        }
        DestroyDog();
    }

    private void OnDogGet(OnDogGet e) {
        gameObject.SetActive(true);
        dogEnvSoundCoroutine = StartCoroutine(DogEnvSound());
        SpawnDog(e.BodyInfo);
    }

    private void SpawnDog(BodyInfo info) {
        if (spawnedDog) {
            return;
        }
        spawnedDog = AlienBody.BuildShadowBody(info, false, "HomeDogBody", 0.6f, 2);
        spawnedDog.transform.position = dogSpawnPosition.position;
        spawnedDog.transform.SetParent(dogSpawnPosition);
        spawnedDog.GetComponent<AlienBody>().ShowColor(0);
    }
    
    private void DestroyDog() {
        if (spawnedDog != null) {
            Destroy(spawnedDog);
        }
    }

    private IEnumerator DogEnvSound() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(30f, 60f));
            if (dogModel.isDogAlive && dogModel.HaveDog  && !dogModel.SentDogBack) {
                AudioSystem.Singleton.Play2DSound($"dog_usual_{Random.Range(1, 3)}");
            }
            
        }
    }
}
