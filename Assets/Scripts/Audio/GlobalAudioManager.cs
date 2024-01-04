using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioManager : LocalAudioManager
{


    public static GlobalAudioManager Instance;

    protected override void Awake()
    {
        if (GlobalAudioManager.Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            base.Awake();
        }

    }
}
