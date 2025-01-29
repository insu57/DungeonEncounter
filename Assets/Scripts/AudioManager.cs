
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class AudioManager : SerializeSingleton<AudioManager>
{
    //진행중 오디오 매니저 싱글톤...
    public enum BGM
    {
        TitleBgm,
        MainBgm,
        Stage1BGM,
    }  
    public enum Sfx 
    {
        ButtonSfx,
        AttackSfx,
        DodgeSfx,
        SkillSfx,
        InventoryOpenSfx,
        ItemPickupSfx,
        MoneyPickupSfx,
        PlayerDamageSfx,
        EnemyDamageSfx,
    }
    
    public enum Voice
    {  
        AttackVoice,
        DamagedVoice,
    }

    public enum Footstep
    {
        RockFootstep,
        ForestFootstep
    }
   
    [BoxGroup("AudioClip Dictionary")]
    //[GUIColor("#ffb900")]
    [OdinSerialize] private Dictionary<BGM, AudioClip> _bgmAudioClips = new Dictionary<BGM, AudioClip>();
    [BoxGroup("AudioClip Dictionary")]
    [OdinSerialize] private Dictionary<Sfx, AudioClip> _sfxAudioClips = new Dictionary<Sfx, AudioClip>();
    [BoxGroup("AudioClip Dictionary")]
    [OdinSerialize] private Dictionary<Voice, AudioClip> _voiceAudioClips = new Dictionary<Voice, AudioClip>();
    [BoxGroup("AudioClip Dictionary")]
    [OdinSerialize] private Dictionary<Footstep, AudioClip> _footstepAudioClips = new Dictionary<Footstep, AudioClip>();
    
    //[GUIColor("#ff5900")]
    [BoxGroup("AudioSource")]
    [SerializeField] private AudioSource audioBgm;
    [BoxGroup("AudioSource")]
    [SerializeField] private AudioSource audioSfx;
    [BoxGroup("AudioSource")]
    [SerializeField] private AudioSource audioFootstep;
    [BoxGroup("AudioSource")]
    [SerializeField] private AudioMixer audioMixer;
    
    private float _bgmVolume;
    private float _sfxVolume;
    
    public void PlayBGM(BGM bgm)
    {
        //_audioBgm.clip = bgms[(int)bgmIdx];
        audioBgm.clip = _bgmAudioClips[bgm];
        audioBgm.Play();
    }

    private void ChangeBGMOnSceneLoad(string sceneName)
    {
        //AudioClip newBGM = null;
        switch (sceneName)
        {
            case LoadingManager.TitleScene:
                PlayBGM(BGM.TitleBgm);
                break;
            case LoadingManager.MainScene:
                PlayBGM(BGM.MainBgm);
                break;
            case LoadingManager.Stage1Scene:
                PlayBGM(BGM.Stage1BGM);
                break;
            
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        //_audioSfx.PlayOneShot(sfxs[(int)sfxIdx]);
        audioSfx.PlayOneShot(_sfxAudioClips[sfx]);
    }

    public void PlayVoice(Voice voice)
    {
        audioSfx.PlayOneShot(_voiceAudioClips[voice]);
    }
    
    public void PlayFootstep(Footstep footstep)
    {
        if (audioFootstep.isPlaying) return;
        //_audioFootstep.clip = footsteps[(int)footstepIdx];
        audioFootstep.clip = _footstepAudioClips[footstep];
        audioFootstep.Play();

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeBGMOnSceneLoad(scene.name);
    }
    
    public override void Awake()
    {
        base.Awake();
        audioBgm = transform.Find("AudioBGM").GetComponent<AudioSource>();
        audioSfx = transform.Find("AudioSfx").GetComponent<AudioSource>();
        audioFootstep = transform.Find("AudioFootstep").GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
