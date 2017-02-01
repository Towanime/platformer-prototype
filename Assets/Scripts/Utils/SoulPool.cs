using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulPool : BasicPool
{
    public static SoulPool instance;

    void Awake()
    {
        instance = this;
    }
}
