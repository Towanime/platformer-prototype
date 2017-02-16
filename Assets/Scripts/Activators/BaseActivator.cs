using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public abstract class BaseActivator : MonoBehaviour
{

    public virtual void Activate(GameObject trigger) {

    }

    public virtual void Desactivate()
    {

    }

}