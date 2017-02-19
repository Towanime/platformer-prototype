using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimationListener : MonoBehaviour
{
    public Renderer renderer;

    void OnBounceDownFrame ()
    {
        if (!renderer.isVisible) return;
        SoundManager.Instance.Play(SoundManager.Instance.catBounceDownSound);
    }
}
