using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public abstract class BaseActivator : MonoBehaviour
{

    public abstract void Activate(GameObject trigger);

    public abstract void Desactivate();

}