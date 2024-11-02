using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    //진행중 오디오 매니저 싱글톤...
    public enum Bgm {
        TitleBgm,
        MainBgm
    }  
    public enum Sfx 
    {
        ButtonSfx,
        AttackSfx,
        RunSfx,
        AttackVoice
    }
    
    public enum Voice
    {  
        AttackVoice,
        DamagedVoice,
    }
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip[] sfxs;
   
    private AudioSource _audioBgm;
    private AudioSource _audioSfx;
    
    [SerializeField] private AudioMixer audioMixer;
    
    private float _bgmVolume;
    private float _sfxVolume;
    
    public void PlayBGM(Bgm bgmIdx)
    {
        _audioBgm.clip = bgms[(int)bgmIdx];
        _audioBgm.Play();
    }

    public void PlaySfx(Sfx sfxIdx)
    {
        _audioSfx.PlayOneShot(sfxs[(int)sfxIdx]);
    }

    public override void Awake()
    {
        base.Awake();
        _audioBgm = transform.Find("AudioBGM").GetComponent<AudioSource>();
        _audioSfx = transform.Find("AudioSfx").GetComponent<AudioSource>();
    }
}
