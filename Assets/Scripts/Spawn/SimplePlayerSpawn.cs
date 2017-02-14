using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the latest player spawn point touched by this trigger.
/// </summary>
public class SimplePlayerSpawn : MonoBehaviour
{
    public GameObject spawnPoint;
    public GameObject player;
    public PlayerInput input;
    public GameObject pitTargetDummy;
    public CameraController cameraController;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spawn Point"))
        {
            // save spawn point
            spawnPoint = other.gameObject;
        } else if (other.CompareTag("Pit"))
        {
            pitTargetDummy.transform.position = transform.position;
            cameraController.target = pitTargetDummy;
            input.enabled = false;
            Invoke("Spawn", 0.5f);
        }
    }

    /// <summary>
    /// Set the dark lord on the position of the latest spawn point.
    /// </summary>
    public void Spawn()
    {
        cameraController.target = player;
        player.transform.position = spawnPoint != null ? spawnPoint.transform.position : new Vector3(0, 1, 0);
        input.enabled = true;
    }
}
