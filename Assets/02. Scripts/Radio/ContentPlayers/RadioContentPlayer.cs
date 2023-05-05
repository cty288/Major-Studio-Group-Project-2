using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RadioContentPlayer : AbstractMikroController<MainGame> {
    public abstract void Mute(bool mute);

    public abstract bool IsPlaying();

    public abstract void Stop();
    
    public abstract void Play(IRadioContent content, Action<RadioContentPlayer> onFinish, bool isMuted);

    public abstract void SetVolume(float relativeVolume, float globalVolume, bool isLoud, bool isInstant);
    
    public abstract void OnGamePaused(bool isPaused);
}
