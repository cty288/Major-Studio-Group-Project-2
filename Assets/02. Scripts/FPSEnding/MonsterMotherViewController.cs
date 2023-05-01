using System;
using System.Collections;
using System.Collections.Generic;
using _02._Scripts.BodyManagmentSystem;
using _02._Scripts.FPSEnding;
using _02._Scripts.GameTime;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.AudioKit;
using MikroFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterMotherViewController : AbstractMikroController<MainGame>, IPointerClickHandler {
	private MonsterMotherModel monsterMotherModel;
	protected Animator animator;
	protected TMP_Text hintText;
	protected PlayerResourceModel playerResourceModel;
	protected AudioSource roarAudio = null;
	
	public Action OnMonsterMotherClicked;
	private void Awake() {
		gameObject.SetActive(false);
		monsterMotherModel = this.GetModel<MonsterMotherModel>();
		monsterMotherModel.isFightingMother.RegisterOnValueChaned(OnFightMotherChanged)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		monsterMotherModel.MotherHealth.RegisterOnValueChaned(OnMonsterMotherHealthChanged)
			.UnRegisterWhenGameObjectDestroyed(gameObject);
		playerResourceModel = this.GetModel<PlayerResourceModel>();
		animator = GetComponent<Animator>();
		hintText = transform.Find("Canvas").GetComponentInChildren<TMP_Text>(true);
	}

	private void OnMonsterMotherHealthChanged(int health) {
		if (!gameObject.activeInHierarchy) {
			return;
		}
		if (health <= 0) {
			animator.SetTrigger("Die");
			hintText.text = "";
			if (roarAudio != null) {
				AudioSystem.Singleton.StopSound(roarAudio);
				roarAudio = null;
				AudioSource dieSource = AudioSystem.Singleton.Play2DSound("monster_die");
				this.Delay(dieSource.clip.length, () => {
					monsterMotherModel.isFightingMother.Value = false;
				});
			}

			this.GetModel<BodyModel>().KillBodyInfo(monsterMotherModel.MotherBodyTimeInfo.BodyInfo);
			
			GameTimeModel gameTimeModel = this.GetModel<GameTimeModel>();
			DateTime currentTime = gameTimeModel.GetDay(gameTimeModel.Day + 1);
			
			
			this.GetSystem<GameEventSystem>().AddEvent(new FPSKillMonsterEndingPhoneEvent(
				new TimeRange(currentTime, currentTime.AddMinutes(60)),
				new FPSKillMonsterEndingPhoneContact(), 6));

		}
		else {
			hintText.text = "Shoot more! " + GetBulletCountText();
		}
	}

	private void OnFightMotherChanged(bool isFighting) {
		gameObject.SetActive(isFighting);
		if (isFighting) {
			hintText.text = "Shoot!" + GetBulletCountText();
			roarAudio = AudioSystem.Singleton.Play2DSound("monster_roar_2");
		}
	}

	public void OnMonsterKillPlayer() {
		this.GetModel<GameStateModel>().GameState.Value = GameState.End;
		DieCanvas.Singleton.Show("You are killed by the monster!", 3);
		if (roarAudio != null) {
			AudioSystem.Singleton.StopSound(roarAudio);
			roarAudio = null;
		}
		AudioSource dieSource = AudioSystem.Singleton.Play2DSound("monster_kill_player");
		
	}
	
	private string GetBulletCountText() {
		int bulletCount = playerResourceModel.GetResourceCount<BulletGoods>();
		if (bulletCount > 0) {
			return $" ({bulletCount} bullets left)";
		}
		else {
			return " (No bullets left!)";
		}
		
	}

	public void OnPointerClick(PointerEventData eventData) {
		OnMonsterMotherClicked?.Invoke();
	}
}
