using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public float teleportSpeed = 3f;
    private bool teleporting;
    private Vector3 toPosition;
    private List<GameObject> souls = new List<GameObject>();
    private CharacterMovement characterMovement;
    private CharacterController characterController;
    private Vector3 lastPosition;

    void Start()
    {
        characterMovement = GetComponentInParent<CharacterMovement>();
        characterController = GetComponentInParent<CharacterController>();
    }

    void FixedUpdate()
    {
        if (!teleporting) return;
        float step = teleportSpeed * Time.fixedDeltaTime;
        Vector3 currentPosition = transform.parent.position;
        if (lastPosition == currentPosition)
        {
            teleporting = false;
        }
        else
        {
            Vector3 nextPosition = Vector3.MoveTowards(currentPosition, toPosition, step);
            characterController.Move(nextPosition - currentPosition);
        }
        lastPosition = currentPosition;
    }

    public bool DoTeleport()
    {
        GameObject nearestSoul = null;
        Vector3 currentPosition = transform.position;
        float minDistance = 0;
        for (int i = 0; i<souls.Count; i++)
        {
            GameObject soulObject = souls[i];
            Vector3 soulPosition = soulObject.transform.position;
            float xDiff = soulPosition.x - currentPosition.x;
            bool soulAhead = xDiff == 0 || Mathf.Sign(xDiff) == characterMovement.LastInputDirection;
            float distance = Vector2.Distance(currentPosition, soulPosition);
            if (soulAhead && (i == 0 || distance < minDistance))
            {
                nearestSoul = soulObject;
                minDistance = distance;
            }
        }
        if (nearestSoul != null)
        {
            teleporting = true;
            toPosition = nearestSoul.transform.position;
        }
        return teleporting;
    }

    public bool HasTarget
    {
        get { return souls.Count > 0; }
    }

    public bool IsTeleporting
    {
        get { return teleporting; }
    }

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

    public void RemoveSoul(GameObject soul)
    {
        souls.Remove(soul);
    }
}
