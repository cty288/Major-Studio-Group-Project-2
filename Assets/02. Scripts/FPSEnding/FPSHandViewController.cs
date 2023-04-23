using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.FPSEnding;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

public class FPSHandViewController : AbstractMikroController<MainGame>
{
	private MonsterMotherModel monsterMotherModel;
	protected Animator animator;
	protected PlayerResourceModel playerResourceModel;
	protected GameStateModel gameStateModel;

	[SerializeField] private MonsterMotherViewController monsterMotherViewController;
	private void Awake() {
		gameObject.SetActive(false);
		monsterMotherModel = this.GetModel<MonsterMotherModel>();
		playerResourceModel = this.GetModel<PlayerResourceModel>();
		monsterMotherModel.isFightingMother.RegisterOnValueChaned(OnFightMotherChanged)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		animator = GetComponent<Animator>();
		playerResourceModel = this.GetModel<PlayerResourceModel>();
		gameStateModel = this.GetModel<GameStateModel>();
		monsterMotherViewController.OnMonsterMotherClicked += OnMonsterMotherClicked;
	}

	private void OnDestroy() {
		monsterMotherViewController.OnMonsterMotherClicked -= OnMonsterMotherClicked;
	}

	private void OnMonsterMotherClicked() {
		if (monsterMotherModel.MotherHealth.Value > 0 && gameStateModel.GameState.Value!=GameState.End) {
			if (playerResourceModel.HasEnoughResource<BulletGoods>(1)) {
				playerResourceModel.RemoveResource<BulletGoods>(1);
				AudioSystem.Singleton.Play2DSound("gun_fire");
				monsterMotherModel.MotherHealth.Value--;
				animator.SetTrigger("Shoot");
			}
			else {
				AudioSystem.Singleton.Play2DSound("gun_no_bullet");
			}
		}
	}

	private void OnFightMotherChanged(bool isFighting) {
		if (isFighting) {
			gameObject.SetActive(isFighting);
		}
		else {
			animator.CrossFade("ShootEnd", 0.02f);
			this.Delay(1f, () => {
				gameObject.SetActive(false);
			});
		}
		
	}
	
}
