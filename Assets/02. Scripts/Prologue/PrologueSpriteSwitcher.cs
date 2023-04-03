using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class PrologueSpriteSwitcher : AbstractMikroController<MainGame> {
	[field: SerializeField] private Sprite prologueSprite;

	private Sprite originalSprite;
	private SpriteRenderer spriteRenderer;
	private void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalSprite = spriteRenderer.sprite;
		this.RegisterEvent<OnNewDay>(OnNewDay).UnRegisterWhenGameObjectDestroyed(gameObject);
	}

	private void OnNewDay(OnNewDay e) {
		if (e.Day == 0) {
			spriteRenderer.sprite = prologueSprite;
		}
		else {
			spriteRenderer.sprite = originalSprite;
		}
	}
}
