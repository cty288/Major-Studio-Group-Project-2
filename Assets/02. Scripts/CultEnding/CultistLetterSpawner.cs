using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.CultEnding;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using Random = UnityEngine.Random;

public class CultistLetterSpawner : AbstractMikroController<MainGame> {
    [SerializeField] protected Vector2 spawnXRanges;

    [SerializeField] protected GameObject letterPrefab;
    private void Awake() {
        this.RegisterEvent<OnCultistLetterSpawned>(OnCultistLetterSpawned)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnCultistLetterSpawned(OnCultistLetterSpawned e) {
        Vector2 spawnPosition = new Vector2(Random.Range(spawnXRanges.x, spawnXRanges.y), 1);
        GameObject letter = Instantiate(letterPrefab, spawnPosition, Quaternion.identity);
        letter.GetComponent<CultLetterUnderDoorViewController>().SetContents(e.contents);
    }
}
