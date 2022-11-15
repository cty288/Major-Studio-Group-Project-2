using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice.Model.Enum;
using DG.Tweening;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.Audio;


public class SentenceInfo {
    public string CompleteSentence;
    public string SentenceFragment;
    public Action Callback;
    public float Rate;
    public SentenceInfo(string completeSentence, string sentenceFragment, Action callback, float rate) {
        this.CompleteSentence = completeSentence;
        this.SentenceFragment = sentenceFragment;
        this.Callback = callback;
        this.Rate = rate;
    }
}

[RequireComponent(typeof(AudioSource))]
public class Speaker :  AbstractMikroController<MainGame> {
    private AudioSource audioSource;
    private AudioMixer audioMixer;

    private Queue<SentenceInfo> sentenceQueues = new Queue<SentenceInfo>();
    private SentenceInfo currentSentence = null;
    
    [SerializeField] private Subtitle subtitile;
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
        Crosstales.RTVoice.Speaker.Instance.OnSpeakCompleted.AddListener(OnSpeakCompleted);
    }

    private void OnDestroy() {
        Crosstales.RTVoice.Speaker.Instance.OnSpeakCompleted.RemoveListener(OnSpeakCompleted);
    }

    private void OnSpeakCompleted(string s) {
        if (sentenceQueues.Count > 0) {
            SentenceInfo nextSentence = sentenceQueues.Dequeue();
            if(currentSentence!=null && currentSentence.CompleteSentence != nextSentence.CompleteSentence) {
                currentSentence.Callback?.Invoke();
            }

            SpeakSentence(nextSentence, nextSentence.Rate);
        }
        else {
            Stop();
        }
    }

    public void Speak(string sentence, Action onEnd = null, float rate = 1f) {
        bool needSpeak = sentenceQueues.Count == 0;
        List<string> splitedSentences = VoiceTextSpliter.Split(sentence);
        foreach (string splitedSentence in splitedSentences) {
            sentenceQueues.Enqueue(new SentenceInfo(sentence, splitedSentence, onEnd, rate));
        }

        if (needSpeak) {
            SentenceInfo text = sentenceQueues.Dequeue();
           
            SpeakSentence(text, rate);
        }
    }

    public void Corrupt(float time, Action onFinished) {
        //change cutoff freq
        audioMixer.DOSetFloat("cutoffFreq", 0, time).OnComplete(() => {
            onFinished?.Invoke();
        });
        audioMixer.DOSetFloat("resonance", 10, time);
    }
    
    private void SpeakSentence(SentenceInfo text, float rate) {
        currentSentence = text;
        Crosstales.RTVoice.Speaker.Instance.Speak(text.SentenceFragment, audioSource, Crosstales.RTVoice.Speaker.Instance.VoiceForGender(Gender.MALE), true, rate);
        if (subtitile) {
            subtitile.OnSpeakStart(text.SentenceFragment);
        }
     
    }

    public void Stop() {
        sentenceQueues.Clear();
        if (subtitile) {
            subtitile.OnSpeakStart("");
        }

        audioMixer.SetFloat("cutoffFreq", 4600);
        audioMixer.SetFloat("resonance", 1);
        audioSource.Stop();
        if (currentSentence!=null) {
            currentSentence.Callback?.Invoke();
        }
        
        currentSentence = null;
    }
}
