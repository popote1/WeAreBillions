using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    
    [SerializeField] private AudioMixerGroup _sfxMixer;
    [SerializeField] private AudioMixerGroup _musicMixer;

    [SerializeField] private Transform _sfxTransform;
    [SerializeField] private Transform _musicTransform;

    [Space(10)] [Header("Debug")] 
    [SerializeField] private AudioClip _music1;
    [SerializeField] private AudioClip _music2;
    [SerializeField] private AudioClip _music3;

    public static AudioManager Instance;

    private List<AudioSource> _sfxAudioSources = new List<AudioSource>();
    private List<ActiveSFXAudioSource>_activeSFXAudioSource = new List<ActiveSFXAudioSource>();
    private List<ActiveMusicAudioSource> _activeMusicAudioSources = new List<ActiveMusicAudioSource>();
    private ActiveMusicAudioSource _activeMusicAudioSource;
    private float _musicFadeTime;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Debug.LogWarning("Try To Have 2 AudioManager at the same time!");
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        ManageActiveSFXAudioSource();
        ManageActiveMusicAudioSource();
    }

    public void PlaySFX(AudioClip audioClip, float volume = 1f, float pitch = 1f)
    {
        AudioSource audioSource = GetSFXAudioSource();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.enabled = true;
        audioSource.Play();
        _activeSFXAudioSource.Add(new ActiveSFXAudioSource(audioSource, audioClip.length+1));
    }

    public void PlayMusic(AudioClip audioClip,float fadingTime=3, float volume = 1f) {
        if (_activeMusicAudioSource != null) {
            foreach (var asMusic in _activeMusicAudioSources) {
                asMusic.Time = fadingTime * asMusic.NormalizeVolume;
                asMusic.IsActiveMusic = false;
            }
        }
        
        AudioSource audioSource = _musicTransform.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.outputAudioMixerGroup = _musicMixer;
        audioSource.volume = 0;
        audioSource.spatialize = false;
        
        audioSource.Play();

        _musicFadeTime = fadingTime;
        
        ActiveMusicAudioSource activeMusicAudioSource = new ActiveMusicAudioSource();
        activeMusicAudioSource.AudioSource = audioSource;
        activeMusicAudioSource.Time = 0;
        activeMusicAudioSource.IsActiveMusic = true;
        _activeMusicAudioSource = activeMusicAudioSource;
        _activeMusicAudioSources.Add(activeMusicAudioSource);
    }

    private void ManageActiveSFXAudioSource() {
        for (int i = _activeSFXAudioSource.Count-1; i >= 0; i--) {
            _activeSFXAudioSource[i].Timer-= Time.deltaTime;
            if (_activeSFXAudioSource[i].Timer <= 0) {
                _activeSFXAudioSource[i].AudioSource.enabled = false;
                _activeSFXAudioSource.Remove(_activeSFXAudioSource[i]);
            }
        }
    }

    private void ManageActiveMusicAudioSource() {
        for (int i = _activeMusicAudioSources.Count-1; i >=0 ; i--) {
            if (_activeMusicAudioSources[i].IsActiveMusic) {
                _activeMusicAudioSources[i].Time += Time.deltaTime;
                if (_activeMusicAudioSources[i].Time >= _musicFadeTime) {
                    _activeMusicAudioSources[i].Time = _musicFadeTime;
                }
            }
            else {
                _activeMusicAudioSources[i].Time -= Time.deltaTime;
                if (_activeMusicAudioSources[i].Time <= 0) {
                    Destroy(_activeMusicAudioSources[i].AudioSource);
                    _activeMusicAudioSources.Remove(_activeMusicAudioSources[i]);
                    continue;
                }
            }
            _activeMusicAudioSources[i].NormalizeVolume = _activeMusicAudioSources[i].Time / _musicFadeTime;
            _activeMusicAudioSources[i].AudioSource.volume = _activeMusicAudioSources[i].NormalizeVolume;
        }
    }

    private AudioSource GetSFXAudioSource() {

        
        foreach (var audioSource in _sfxAudioSources) {
            if (audioSource.enabled) continue;
            return audioSource;
        }
        

        //Create new audio source 
        AudioSource newAudioSource = _sfxTransform.AddComponent<AudioSource>();
        newAudioSource.outputAudioMixerGroup = _sfxMixer;
        newAudioSource.playOnAwake = false;
        newAudioSource.spatialize = false;
        _sfxAudioSources.Add(newAudioSource);
        return newAudioSource;
    }

    private class ActiveSFXAudioSource {
        public AudioSource AudioSource;
        public float Timer;
        public ActiveSFXAudioSource(AudioSource audioSource, float timer) {
            AudioSource = audioSource;
            Timer = timer;
        }
    }

    private class ActiveMusicAudioSource
    {
        public AudioSource AudioSource;
        public float NormalizeVolume;
        public float Time;
        public bool IsActiveMusic;
    }

    [ContextMenu("TestPlayingMusic1")]
    private void TestPlayerMusic1() {
        PlayMusic(_music1, 1);
    }
    [ContextMenu("TestPlayingMusic2")]
    private void TestPlayerMusic2() {
        PlayMusic(_music2, 5);
    }
    [ContextMenu("TestPlayingMusic3")]
    private void TestPlayerMusic3() {
        PlayMusic(_music3, 10);
    }
    
}
