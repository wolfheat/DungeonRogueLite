using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Wolfheat.StartMenu
{
    public enum WallSoundType { Stone, Moss, Flesh, Sand } 
    public enum SoundName
    {
        MenuStep, MenuError, MenuClick, MenuOver, PunchRock,
        HitEnemy, ExitStairReached, EnemyDie
        //FireSound,
        //FireContinious,
        //RockExplosion,
        //DieByFire,
        //StabEnemy,
        //KillEnemy,
        //PickUp,
        //SkeletonDie,
        //EnemyGetHit,
        //Hissing,
        //Miss,
        //HitStone,
        //CrushStone,
        //None,
        ////PowerUpDamage,
        //PowerUpSpeed,
        //BoomPlayerDies,
        //PickUpHeart,
        //SkeletonBuildUpAttack,
        //Coin,
        //OpenDoor,
        //LockedDoor,
        //PickUpMap,
        //PickUpKey,
        //EnemyHitGroundEffect,
        //OpenDoorHitWall,
        //CantDoThatSound,
        //GemPickup,
        //Teleport,
        //BossDying,
        //CantAfford
    }

    public enum MusicName {MenuMusic, OutDoorMusic, IndoorMusic, DeadMusic, CreditsMusic, BossMusic}

    [Serializable]
    public class Music : BaseSound
    {
        public MusicName name;
        public void SetSound(AudioSource source)
        {
            audioSource = source;
        }
    }

    [Serializable]
    public class Sound: BaseSound
    { 
        public SoundName name;
        public void SetSound(AudioSource source)
        {
            audioSource = source;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loop;
        }
    }
    
    [Serializable]
    public class BaseSound
        {
        public AudioClip clip;
        [Range(0f,1f)]
        public float volume;
        [Range(0.8f, 1.2f)]
        public float pitch=1f;
        public bool loop=false;
        [HideInInspector] public AudioSource audioSource;

    }

    public class SoundMaster : MonoBehaviour
    {
        public static SoundMaster Instance { get; private set; }
        public const float MuteBoundary = 0.015f;
        public AudioMixer mixer;
        public AudioMixerGroup masterGroup;  
        public AudioMixerGroup musicMixerGroup;  
        public AudioMixerGroup SFXMixerGroup;  
        [SerializeField] private Sound[] sounds;
        [SerializeField] private Sound[] effects;
        [SerializeField] private Music[] musics;

        [Header("Footsteps")]
        [SerializeField]private AudioClip[] footstepMoss;



        [Header("Other")]
        //[SerializeField]private AudioClip[] coins;
        //[SerializeField]private AudioClip[] getHit;
        //[SerializeField]private AudioClip[] bossGetHit;
        [SerializeField]private AudioClip[] playerHitWithSword;
        //[SerializeField]private AudioClip[] pickAxeHitMoss;
        //[SerializeField]private AudioClip[] pickAxeHitFlesh;
        //[SerializeField]private AudioClip[] pickAxeHitSand;
        //[SerializeField]private AudioClip[] pickAxeCrushStone;
        //[SerializeField] private AudioClip[] pickAxeCrushMoss;
        //[SerializeField] private AudioClip[] pickAxeCrushFlesh;
        //[SerializeField] private AudioClip[] pickAxeCrushSand;
        //
        //
        //
        //[SerializeField]private AudioClip[] footstepFlesh;
        //[SerializeField]private AudioClip[] footstepSand;
        //[SerializeField]private AudioClip[] footstepStone;
        //[SerializeField]private AudioClip[] footstepIndoor;
        //[SerializeField]private AudioClip[] footstepAltar;

        private Dictionary<SoundName,Sound> soundsDictionary = new();
        private Dictionary<MusicName,Music> musicDictionary = new();

        AudioSource musicSource;
        MusicName activeMusic = MusicName.MenuMusic;
        AudioSource audioSource;
        AudioSource stepSource;
        AudioSource getHitSource;

        SoundSettings soundSettings = new SoundSettings();

        public Action GlobalMuteChanged;

        private void OnEnable()
        {            
            //SavingUtility.LoadingComplete += LoadingComplete;
        }

        private void LoadingComplete()
        {
            Debug.Log("Loading of settings complete setting the soundSettings");

            //soundSettings = SavingUtility.gameSettingsData.soundSettings;
        }

        ///private void Start()
        ///{
        ///        var src = gameObject.AddComponent<AudioSource>();
        ///        src.outputAudioMixerGroup = SFXMixerGroup;
        ///        src.clip = musics[0].clip;
        ///        src.Play();
        ///
        ///        Debug.Log("Testing raw audio playback: " + src.clip.name);
        ///}
        ///
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            Debug.Log("SoundMaster Start");        

            // Define all sounds
            foreach (var sound in sounds)
            {
                if (sound.clip == null) continue;
                sound.SetSound(gameObject.AddComponent<AudioSource>());
                sound.audioSource.outputAudioMixerGroup = SFXMixerGroup;
                soundsDictionary.Add(sound.name, sound);
            }
            foreach (var sound in effects)
            {
                if (sound.clip == null) continue;
                sound.SetSound(gameObject.AddComponent<AudioSource>());
                sound.audioSource.outputAudioMixerGroup = SFXMixerGroup;
                soundsDictionary.Add(sound.name, sound);
            }

            //Audio
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 0.5f;
            audioSource.outputAudioMixerGroup = SFXMixerGroup;
            
            //Steps
            stepSource = gameObject.AddComponent<AudioSource>();
            stepSource.volume = 0.5f;
            stepSource.outputAudioMixerGroup = SFXMixerGroup;

            // And Music
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.volume = 1f;
            musicSource.outputAudioMixerGroup = musicMixerGroup;

            foreach (var music in musics)
            {
                if (music.clip == null) continue;
                // All music use same source (since only one will be playing at a time)
                music.SetSound(musicSource);
                musicDictionary.Add(music.name, music);
            }

            StartCoroutine(DelayedStartMusic());

            // Set Volumes            
            UpdateVolume();

            //Inputs.Instance.PlayerControls.Player.M.performed += MusicToggle;
        }

        private void MusicToggle(InputAction.CallbackContext context)
        {
            Debug.Log("Toggle Music");
            PlayMusic(activeMusic);
        }

        private IEnumerator DelayedStartMusic()
        {
            Debug.Log("Delayed Music playing in 0.2s");
            yield return new WaitForSeconds(0.2f);
            PlayMusic(MusicName.BossMusic);
        }

        //private bool haveNotSaidExplode = true;
        //public void BombHissing()
        //{
        //    if (haveNotSaidExplode)
        //    {
        //        //PlaySound(SoundName.ItsGonaBlow);
        //        haveNotSaidExplode= false;
        //    }
        //    PlaySound(SoundName.Hissing);
        //}
        public void AddRestartSpeech()
        {
            StartCoroutine(AddRestartSpeechCO());
        }
        public IEnumerator AddRestartSpeechCO()
        {
            yield return new WaitForSeconds(1.5f);
            //PlaySpeech(SoundName.MyHeadHurts);
            yield return new WaitForSeconds(1.5f);
            //PlaySpeech(SoundName.INeedToBeMoreCareful);
        }

        public void PlayMusic(MusicName name)
        {
            if (activeMusic == name) return;

            Debug.Log("PLAY MUSIC "+name);
            activeMusic = name; // Leave this here so the correct music that should be played is still updated if music is reenabled

            //if (!soundSettings.GlobalMaster || !soundSettings.UseMaster || !soundSettings.UseMusic || !SavingUtility.HasLoaded) return;

            if (musicDictionary.ContainsKey(name))
            {
                if (musicDictionary[name].audioSource.isPlaying && !musicDictionary[name].loop)
                    return;

                musicSource.clip = musicDictionary[name].clip;
                musicSource.volume = musicDictionary[name].volume;
                musicSource.pitch = musicDictionary[name].pitch;
                musicSource.loop = musicDictionary[name].loop;
                musicSource.Play();
            }
            else
                Debug.LogWarning("No clip named "+name+" in dictionary.");

        }

        private void Update()
        {
            if (speechQueue.Count > 0)
            {
                if (speechQueue[0].isPlaying)
                    return;
                speechQueue.RemoveAt(0);
                // At least one speech to play
                if (speechQueue.Count >= 1)
                    speechQueue[0].Play();
            }
        }

        private List<AudioSource> speechQueue = new List<AudioSource>();

        public void PlaySound(SoundName name, bool allowInterupt= true)
        {
            //Debug.Log("Play Sound "+name);
            if (!soundSettings.GlobalMaster || !soundSettings.UseMaster || !soundSettings.UseSFX) return;

            if (soundsDictionary.ContainsKey(name))
            {
                if (!allowInterupt && soundsDictionary[name].audioSource.isPlaying && !soundsDictionary[name].loop)
                    return;

                //Debug.Log("Start Sound: "+name);
                if(soundsDictionary[name].audioSource == null)
                    Debug.Log("Sound is null: "+name);

                //Debug.Log("volume = " + soundsDictionary[name].volume); 
                //Debug.Log("pitch = " + soundsDictionary[name].pitch); 
                
                soundsDictionary[name].audioSource.Play();
                //soundsDictionary[name].audioSource.PlayOneShot(soundsDictionary[name].clip);

                // Try playing it throiugh the extra audioSource instead
                //audioSource.clip = soundsDictionary[name].clip;
                //audioSource.Play();
            }
            else 
                Debug.LogWarning("No clip named "+name+" in dictionary.");

        }

        public void FadeMusic(float time = 1f)
        {
            StartCoroutine(MusicFade(time));
        }
        public IEnumerator MusicFade(float time)
        {
            float changePerSecond = musicSource.volume / time;
            while (musicSource.volume > 0)
            {
                musicSource.volume -= changePerSecond * Time.deltaTime;
                yield return null;
            }
            musicSource.Stop();
        }
        public void UpdateVolume()
        {

            //Debug.Log($"SoundSettings - GlobalMaster: {soundSettings.GlobalMaster}, UseMaster: {soundSettings.UseMaster}, UseMusic: {soundSettings.UseMusic}");

            Debug.Log("SOUNDMASTER - Updating SoundMaster's Volumes, This uses Sound Settings values "+ soundSettings.MasterVolume+"/"+ soundSettings.MusicVolume+""+ soundSettings.SFXVolume);
            //Convert to dB
            mixer.SetFloat("Volume", Mathf.Log10(soundSettings.MasterVolume) * 20);
        
            //Set Music
            mixer.SetFloat("MusicVolume", Mathf.Log10(soundSettings.MusicVolume) * 20);

            Debug.Log("MusicVolume set to "+(Mathf.Log10(soundSettings.MusicVolume) * 20)+"dB");

            //Set SFX
            mixer.SetFloat("SFXVolume", Mathf.Log10(soundSettings.SFXVolume) * 20);

            EnableSoundAccordingToMixersVolumes();
            
        }

        private void EnableSoundAccordingToMixersVolumes()
        {
            Debug.Log("SOUNDMASTER - Enabling Sound According to Mixer Volumes, Music volume = " + soundSettings.MusicVolume);
            //Master
            soundSettings.UseMaster = soundSettings.MasterVolume > MuteBoundary;
            soundSettings.UseMusic  = soundSettings.MusicVolume > MuteBoundary;
            soundSettings.UseSFX    = soundSettings.SFXVolume > MuteBoundary;
            
            Debug.Log("SOUNDMASTER - Global Sound:" + (soundSettings.GlobalMaster==true?"ON":"OFF") + "    Master: "+ soundSettings.UseMaster + " Music: " +soundSettings.UseMusic+" SFX: "+ soundSettings.UseSFX);

            if (soundSettings.GlobalMaster && soundSettings.UseMaster && soundSettings.UseMusic)
            {
                //Debug.Log("SOUNDMASTER - Global and Master and Music is ON");
                if (!musicSource.isPlaying)
                {
                    Debug.Log("SOUNDMASTER - Resume Music");
                    ResumeMusic();
                }
            }
            else
            {
                //Debug.Log("SOUNDMASTER - Global or Master or Music is OFF");
            
                if (musicSource.isPlaying)
                    musicSource.Stop();
                //Stop all SFX?
            }
            
        }

        //public void ToggleMusic()
        //{
        //    //Debug.Log("TOGGLE MUSIC");
        //    
        //    //Debug.Log("Global Sound Set To:"+ (soundSettings.GlobalMaster==true?"ON":"OFF") + " Master: "+ soundSettings.UseMaster + " Music: " +soundSettings.UseMusic+" SFX: "+ soundSettings.UseSFX);
        //    // Update Music playing 
        //    if (soundSettings.GlobalMaster)
        //    {
        //        //Debug.Log("GLobal master is on");
        //        if (soundSettings.UseMaster && soundSettings.UseMusic && !musicSource.isPlaying)
        //            ResumeMusic();
        //        else
        //            musicSource.Stop();
        //    }
        //    else 
        //        if (musicSource.isPlaying)
        //        {
        //                //Debug.Log("Stopping Music from playing?");
        //            musicSource.Stop();
        //        }
        //    GameSettingsData.GameSettingsUpdated?.Invoke();
        //}   
        public void StopSound(SoundName name)
        {
            if (soundsDictionary.ContainsKey(name))
            {
                soundsDictionary[name].audioSource.Stop();
            }
            else
                Debug.LogWarning("No clip named " + name + " in dictionary.");
        }
        public void ResetMusic()
        {
            ResumeMusic();
        }
        public void ResumeMusic()
        {
            Debug.Log("Resume Music "+activeMusic);
            PlayMusic(activeMusic);
        }

        public void PlayStepSound(int stepSoundType = 0)
        {
           if (!soundSettings.GlobalMaster || !soundSettings.UseMaster || !soundSettings.UseSFX) return;

            switch (stepSoundType)
            {
                case 0:
                    //stepSource.PlayOneShot(footstepFlesh[Random.Range(0, footstepFlesh.Length)]);
                    break;
                case 1:
                    //stepSource.PlayOneShot(footstepSand[Random.Range(0, footstepSand.Length)]);
                    break;
                case 2:
                    //stepSource.PlayOneShot(footstepStone[Random.Range(0, footstepStone.Length)]);
                    break;
                case 3:
                    stepSource.PlayOneShot(footstepMoss[Random.Range(0, footstepMoss.Length)]);
                    break;
                case 4: 
                    //stepSource.PlayOneShot(footstepAltar[Random.Range(0, footstepAltar.Length)]);
                    break;
                case 5:
                    //stepSource.PlayOneShot(footstepIndoor[Random.Range(0, footstepIndoor.Length)]);
                    break;

                default:
                    break;
            }
            // Only play foot step if last footstep is finished playing
            //if (!stepSource.isPlaying)
        }

        public void ReadDataFromSave()
        {
            //Debug.Log("Updating Sound Volumes from saved file");
            UpdateVolume();
        }

        public void PlayWeaponHitEnemy()
        {
            Debug.Log("Play Sound layer hit enemy");
            audioSource.PlayOneShot(playerHitWithSword[Random.Range(0, playerHitWithSword.Length)]);
            //PlaySound(SoundName.HitEnemy);
        }

        public void PlayWeaponKillsEnemy()
        {
            //PlaySound(SoundName.KillEnemy);
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }
    }
}
