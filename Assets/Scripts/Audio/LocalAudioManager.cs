using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocalAudioManager : MonoBehaviour
{
    [SerializeField]
    private SoundEffect[] _soundsToAdd;

    protected Dictionary<string,SoundEffect> _sounds = new Dictionary<string, SoundEffect>();


    protected virtual void Awake()
    {
        foreach (var soundEffect in _soundsToAdd)
        {
            _sounds.Add(soundEffect.Name,soundEffect);
            CreateSource(soundEffect);
        }
    }

    protected void CreateSource(SoundEffect sound)
    {
        sound.source = this.AddComponent<AudioSource>();
        sound.source.volume = sound.Volume;
        sound.source.clip = sound.Clip;
        sound.source.playOnAwake = false;
        sound.source.loop = sound.Looping;
        sound.source.spatialBlend = sound.Is3dSound ? 1 : 0;
        sound.source.maxDistance = sound.MaxDistance;
        sound.source.rolloffMode = AudioRolloffMode.Linear;
    }

    public void PlaySound(string name)
    {
        if (_sounds.TryGetValue(name,out SoundEffect effect))
        {
            float pitch = UnityEngine.Random.Range(effect.PitchVariation.x, effect.PitchVariation.y);
            effect.source.pitch = pitch;
            effect.source.Play();
        }
    }

    public void StopSound(string name)
    {
        if (_sounds.TryGetValue(name, out SoundEffect effect))
        {
            effect.source.Stop();
        }
    }
}
