using System;
using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice;
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
    public Gender SpeakGender;
    public float volumeMultiplier;
    public AudioMixerGroup Mixer;
    public string SpeakName;
    public SentenceInfo(string completeSentence, string sentenceFragment, string speakName,AudioMixerGroup mixer, Action callback, float rate, Gender speakGender, float volumeMultiplier=1) {
        this.CompleteSentence = completeSentence;
        this.SentenceFragment = sentenceFragment;
        this.Callback = callback;
        this.Rate = rate;
        this.SpeakGender = speakGender;
        this.volumeMultiplier = volumeMultiplier;
        this.Mixer = mixer;
        this.SpeakName = speakName;
    }
}

[RequireComponent(typeof(AudioSource))]
public class Speaker :  AbstractMikroController<MainGame> {
    private AudioSource audioSource;

    public AudioSource AudioSource => audioSource;
    //private AudioMixer audioMixer;

    public AudioMixer AudioMixer {
        get {
            if (audioSource.outputAudioMixerGroup) {
                return audioSource.outputAudioMixerGroup.audioMixer;
            }

            return null;
        }
    }

    private Queue<SentenceInfo> sentenceQueues = new Queue<SentenceInfo>();
    private SentenceInfo currentSentence = null;
    
    [SerializeField] private Subtitle subtitile;

    public bool IsSpeaking = false;

    private bool inited = false;

    private string currentSpeachUID = "";
    
    private void Awake() {
        audioSource = GetComponent<AudioSource>();

   
        
        
        Crosstales.RTVoice.Speaker.Instance.OnSpeakCompleted.AddListener(OnSpeakCompleted);
        Destroy(GetComponent<LiveSpeaker>());
    }

    private void OnDestroy() {
        Crosstales.RTVoice.Speaker.Instance.OnSpeakCompleted.RemoveListener(OnSpeakCompleted);
    }

    private void OnSpeakCompleted(string s) {
        if (!inited || !IsSpeaking) {
            return;
        }

        if (s != currentSpeachUID) {
            return;
        }
        if (sentenceQueues.Count > 0) {
            SentenceInfo nextSentence = sentenceQueues.Dequeue();
            if(currentSentence!=null && currentSentence.CompleteSentence != nextSentence.CompleteSentence) {
                currentSentence.Callback?.Invoke();
            }

            SpeakSentence(nextSentence, nextSentence.Rate, nextSentence.volumeMultiplier);
        }
        else {
            Stop();
        }
    }

    public void Speak(string sentence, AudioMixerGroup mixer, string speakName, Action onEnd = null, float rate = 1f, float volumeMultiplier = 1f, Gender gender = Gender.MALE ) {
        inited = true;
        IsSpeaking = true;
        bool needSpeak = sentenceQueues.Count == 0;
        List<string> splitedSentences = VoiceTextSpliter.Split(sentence);
        foreach (string splitedSentence in splitedSentences) {
            sentenceQueues.Enqueue(new SentenceInfo(sentence, splitedSentence,speakName,  mixer, onEnd, rate, gender, volumeMultiplier));
        }

        if (needSpeak) {
            SentenceInfo text = sentenceQueues.Dequeue();
            if (AudioMixer) {
                AudioMixer.SetFloat("cutoffFreq", 4600);
                AudioMixer.SetFloat("resonance", 1);
            }
           
            SpeakSentence(text, rate, volumeMultiplier);
        }
    }

    public void Corrupt(float time, Action onFinished) {
        if (!AudioMixer) {
            return;
        }
        //change cutoff freq
        AudioMixer.DOSetFloat("cutoffFreq", 0, time).OnComplete(() => {
            onFinished?.Invoke();
        });
        AudioMixer.DOSetFloat("resonance", 10, time);
    }
    private string RemoveRichTextTags(string text) {
        //find the first < and the first > and remove everything in between. Repeat until no more < or > are found.
        int firstOpen = text.IndexOf("<");
        int firstClose = text.IndexOf(">");
        while (firstOpen != -1 && firstClose != -1)
        {
            text = text.Remove(firstOpen, firstClose - firstOpen + 1);
            firstOpen = text.IndexOf("<");
            firstClose = text.IndexOf(">");
        }
        return text;
    }
    private void SpeakSentence(SentenceInfo text, float rate, float volumeMultiplier) {
        currentSentence = text;
        float volume = text.SpeakGender == Gender.MALE ? 0.5f : 0.8f;
        volume *= volumeMultiplier;
        audioSource.outputAudioMixerGroup = text.Mixer;

        string processedSentence = text.SentenceFragment;
        //remove all rich text tags and all texts in <>
        processedSentence = RemoveRichTextTags(processedSentence);


        currentSpeachUID = Crosstales.RTVoice.Speaker.Instance.Speak(processedSentence, audioSource, Crosstales.RTVoice.Speaker.Instance.VoiceForGender(text.SpeakGender), true, rate,1, volume);
        audioSource.volume = volume;
        if (subtitile) {
            subtitile.OnSpeakStart(text.SentenceFragment, text.SpeakName);
        }
     
    }

    public void Stop() {
        sentenceQueues.Clear();
        
        if (subtitile) {
            subtitile.OnSpeakStart("","");
        }
        IsSpeaking = false;
        if (AudioMixer) {
            AudioMixer.SetFloat("cutoffFreq", 4600);
            AudioMixer.SetFloat("resonance", 1);
        }
       
        audioSource.Stop();
        if (currentSentence!=null) {
            currentSentence.Callback?.Invoke();
        }
        
        currentSentence = null;
        
        
    }
}
