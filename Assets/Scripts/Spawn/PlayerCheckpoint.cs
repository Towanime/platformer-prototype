using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the latest player spawn point touched by this trigger.
/// </summary>
public class PlayerCheckpoint : MonoBehaviour
{
    [Tooltip("Empty game object with the position where the player will show after death.")]
    public Transform checkpoint;
    [Tooltip("Dark lord game object.")]
    public GameObject player;
    [Tooltip("How many seconds it will take to spawn the player in the last checkpoint.")]
    public float spawnTime;
    public bool spawning;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spawn Point"))
        {
            // save spawn point
            checkpoint = other.transform.Find("Spawn");
        } else if (other.CompareTag("Pit"))
        {
            player.GetComponent<ActionStateMachine>().OnFall();
        }
    }

    public void BeginSpawn()
    {
        spawning = true;
        Invoke("Spawn", spawnTime);
    }

    /// <summary>
    /// Set the dark lord on the position of the latest spawn point.
    /// </summary>
    public void Spawn()
    {
        player.transform.position = checkpoint != null ? checkpoint.transform.position : new Vector3(0, 1, 0);
        spawning = false;
    }

    public bool IsSpawning
    {
        get { return spawning; }
    }
}
