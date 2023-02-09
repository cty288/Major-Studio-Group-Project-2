using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerList : MonoMikroSingleton<AudioMixerList> {
    public List<AudioMixerGroup> AudioMixerGroups = new List<AudioMixerGroup>();
    
    public List<AudioMixerGroup> AlienVoiceGroups = new List<AudioMixerGroup>();
}
