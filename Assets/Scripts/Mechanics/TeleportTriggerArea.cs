using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTriggerArea : MonoBehaviour
{
    private List<GameObject> souls = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Soul Drop"))
        {
            souls.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Soul Drop"))
        {
            souls.Remove(other.gameObject);
        }
    }

    public void RemoveSoulFromArea(GameObject soul)
    {
        souls.Remove(soul);
    }

    public List<GameObject> Souls
    {
        get { return souls; }
    }
}
