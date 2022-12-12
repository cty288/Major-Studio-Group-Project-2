using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogFoodViewController : AbstractMikroController<MainGame> {
    [SerializeField] private Sprite dogDieSprite;
    private SpriteRenderer spriteRenderer;
    private TMP_Text infoText;

    private Coroutine dogEnvSoundCoroutine = null;
    private void Awake() {
        this.RegisterEvent<OnDogGet>(OnDogGet).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnDogDie>(OnDogDie).UnRegisterWhenGameObjectDestroyed(gameObject);
        spriteRenderer = GetComponent<SpriteRenderer>();
        infoText = transform.Find("Canvas/Text").GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    private void OnDogDie(OnDogDie e) {
        infoText.text = "My friend is gone...";
        spriteRenderer.sprite = dogDieSprite;
        if (dogEnvSoundCoroutine != null) {
            StopCoroutine(dogEnvSoundCoroutine);
            dogEnvSoundCoroutine = null;
        }
    }

    private void OnDogGet(OnDogGet e) {
        gameObject.SetActive(true);
        dogEnvSoundCoroutine = StartCoroutine(DogEnvSound());
    }

    private IEnumerator DogEnvSound() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(30f, 60f));
            AudioSystem.Singleton.Play2DSound($"dog_usual_{Random.Range(1, 3)}");
        }
    }
}
