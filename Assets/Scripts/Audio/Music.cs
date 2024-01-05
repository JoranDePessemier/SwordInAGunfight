using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public enum MusicState
{
    FadeIn,
    Playing,
    FadeOut,
    Silent
}

[System.Serializable]
public class Music
{
    [SerializeField]
    private string _name;

    public string Name => _name;

    [SerializeField]
    private AudioClip _intro;

    [SerializeField]
    private AudioClip _loop;

    [SerializeField]
    private AudioClip _outro;

    public AudioClip Loop => _loop;

    public AudioClip Outro => _outro;

    public AudioClip Intro => _intro;

    [SerializeField]
    [Range(0f, 1f)]
    private float _volume = 1f;

    public float Volume => _volume;

    public AudioSource source { get; set; }

    public MusicState state { get; set; } = MusicState.Silent;


}
