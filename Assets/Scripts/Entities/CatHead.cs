using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHead : MonoBehaviour
{
    void OnDeath()
    {
        SoundManager.Instance.Play(SoundManager.Instance.catDeathSound);
    }
}
