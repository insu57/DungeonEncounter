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
        MainBgm,
        Stage1Bgm
    }  
    public enum Sfx 
    {
        ButtonSfx,
        AttackSfx,
        RunSfx
    }
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip[] sfxs;
    [SerializeField] private AudioSource audioBgm;
    [SerializeField] private AudioSource audioSfx;
    
    [SerializeField] private AudioMixer audioMixer;
    
    private float _bgmVolume;
    private float _sfxVolume;
    
    public void PlayBGM(Bgm bgmIdx)
    {
        audioBgm.clip = bgms[(int)bgmIdx];
        audioBgm.Play();
    }

    public void PlaySfx(Sfx sfxIdx)
    {
        audioSfx.PlayOneShot(sfxs[(int)sfxIdx]);
    }

    public override void Awake()
    {
        base.Awake();
        //audioBgm = transform.Find("").GetComponent<AudioSource>();
        audioSfx = GetComponent<AudioSource>();
    }
    private void Start()
    {
        //
        
    }
}
