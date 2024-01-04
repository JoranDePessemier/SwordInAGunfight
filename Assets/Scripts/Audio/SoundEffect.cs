using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundEffect 
{
    [SerializeField]
    private string _name;

    public string Name => _name;


    [SerializeField]
    private AudioClip _clip;

    public AudioClip Clip => _clip;

    [SerializeField]
    private float _volume = 1f;

    public float Volume => _volume;

    [SerializeField]
    private bool _looping;

    public bool Looping => _looping;

    [SerializeField]
    private Vector2 _pitchVariation = Vector2.one;

    public Vector2 PitchVariation => _pitchVariation;

    [SerializeField]
    private bool _is3dSound;

    public bool Is3dSound => _is3dSound;

    [SerializeField]
    private float _maxDistance;

    public float MaxDistance => _maxDistance;   

    public AudioSource source { get; set; }
}
