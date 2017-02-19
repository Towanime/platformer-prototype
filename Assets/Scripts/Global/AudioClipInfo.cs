using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioClipInfo {

    public AudioClip audioClip;
    [Range(0.0f, 1.0f)]
    public float volume = 1;
}
