using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    public enum BGM {
        TitleBGM,
        MainBGM,
        Stage1BGM
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

    public void PlayBGM(BGM bgmIdx)
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
        GameObject bgmObject = new GameObject("bgmPlayer");
        bgmObject.transform.parent = transform;
        audioBgm = bgmObject.AddComponent<AudioSource>();
    }
    void Update()
    {
        //if(Scene
        if (SceneManager.loadedSceneCount == 0)
        {
            PlayBGM(BGM.TitleBGM);
        }
        
    }
}
