using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    private Music[] _musicInGame;

    [SerializeField]
    private float _standardFadeSpeed;

    private string _activeMusic;

    private void Awake()
    {
        if (MusicManager.Instance != null)
        {        
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        foreach (var music in _musicInGame)
        {
            CreateSource(music);
        }
    }

    private void CreateSource(Music music)
    {
        music.source = this.AddComponent<AudioSource>();
        music.source.volume = music.Volume;
        music.source.clip = music.Loop;
        music.source.loop = true;
        music.source.playOnAwake = false;
    }

    private void StartFadeIn(string name,float fadeSpeed)
    {
        Music neededMusic = FindMusicByName(name);

        neededMusic.source.volume = 0f;

        if (neededMusic.Intro != null)
        {
            neededMusic.source.clip = neededMusic.Intro;
            neededMusic.source.loop = false;
            neededMusic.source.Play();
            if (neededMusic.Loop != null)
            {
                StartCoroutine(PlayLoopAfterIntro(neededMusic));
            }
        }
        else
        {
            neededMusic.source.clip = neededMusic.Loop;
            neededMusic.source.loop = true;
            neededMusic.source.Play();
        }

        _activeMusic = neededMusic.Name;

        StartCoroutine(FadeIn(neededMusic, fadeSpeed,null));
    }

    private IEnumerator PlayLoopAfterIntro(Music neededMusic)
    {
        yield return new WaitForSecondsRealtime(neededMusic.Intro.length);

        neededMusic.source.clip = neededMusic.Loop;
        neededMusic.source.loop = true;
        neededMusic.source.Play();
    }

    private void StartFadeIn(string name) => StartFadeIn(name,_standardFadeSpeed);

    private void StartFadeOut(string name, float fadeSpeed)
    {
        Music neededMusic = FindMusicByName(name);

        if(neededMusic.Outro != null)
        {
            neededMusic.source.clip = neededMusic.Outro;
            neededMusic.source.loop = false;
            neededMusic.source.Play();
        }

        StartCoroutine(FadeOut(neededMusic, fadeSpeed, () => neededMusic.source.Stop()));
    }

    private void StartFadeOut(string name) => StartFadeOut(name, _standardFadeSpeed);

    public void ChangeMusic(string name)
    {
        if(FindMusicByName(_activeMusic) != null)
        {
            StartFadeOut(_activeMusic);
        }

        StartFadeIn(name);
    }

    private Music FindMusicByName(string name)
    {
        Music neededMusic = null;

        foreach (Music music in _musicInGame)
        {
            if (music.Name == name)
            {
                neededMusic = music;
            }
        }

        return neededMusic;
    }

    private  IEnumerator FadeOut(Music music, float fadeSpeed, Action onComplete)
    {
        music.state = MusicState.FadeOut;
        while (music.source.volume > 0 && music.state == MusicState.FadeOut)
        {
            music.source.volume = Mathf.MoveTowards(music.source.volume,0, fadeSpeed * Time.unscaledDeltaTime);
            yield return 0;
        }
        if (music.state == MusicState.FadeOut)
        {
            music.state = MusicState.Silent;
            onComplete?.Invoke();
        }
    }

    private  IEnumerator FadeIn(Music music, float fadeSpeed, Action onComplete)
    {
        music.state = MusicState.FadeIn;
        
        while (music.source.volume < music.Volume && music.state == MusicState.FadeIn)
        {
            music.source.volume = Mathf.MoveTowards(music.source.volume, music.Volume, fadeSpeed * Time.unscaledDeltaTime);
            yield return 0;
        }
        if(music.state == MusicState.FadeIn)
        {
            music.state = MusicState.Playing;
            onComplete?.Invoke();
        }
    }

    internal void StopAllMusic()
    {
        foreach(Music music in _musicInGame)
        {
            if (music.source.isPlaying)
            {
                StartFadeOut(music.Name);
                _activeMusic = null;
            }
    
        }
    }
}
