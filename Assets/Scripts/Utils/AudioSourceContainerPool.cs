using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceContainerPool : BasicPool
{
    public static AudioSourceContainerPool instance;

    void Awake()
    {
        instance = this;
    }
}
