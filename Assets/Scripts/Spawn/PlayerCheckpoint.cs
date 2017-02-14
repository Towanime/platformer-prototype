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
    [Tooltip("Global player input component, used to disable player input when they die.")]
    public PlayerInput input;
    [Tooltip("Camera controller, this component will target a dummy when the player falls into a pit.")]
    public CameraController cameraController;
    [Tooltip("How many seconds it will take to spawn the player in the last checkpoint.")]
    public float spawnTime;
    /// <summary>
    /// Temporal game object for the camera to target when a player touches a pit only.
    /// </summary>
    private GameObject pitTargetDummy;

    void Start()
    {
        pitTargetDummy = new GameObject();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spawn Point"))
        {
            // save spawn point
            checkpoint = other.transform.Find("Spawn");
        } else if (other.CompareTag("Pit"))
        {
            pitTargetDummy.transform.position = transform.position;
            cameraController.target = pitTargetDummy;
            input.enabled = false;
            Invoke("Spawn", spawnTime);
        }
    }

    /// <summary>
    /// Set the dark lord on the position of the latest spawn point.
    /// </summary>
    public void Spawn()
    {
        cameraController.target = player;
        player.transform.position = checkpoint != null ? checkpoint.transform.position : new Vector3(0, 1, 0);
        input.enabled = true;
    }
}
