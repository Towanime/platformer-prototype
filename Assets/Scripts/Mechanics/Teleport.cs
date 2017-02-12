using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    [Tooltip("Area that contains the souls nearby.")]
    public TeleportTriggerArea teleportTriggerArea;
    [Tooltip("Object with the same dimentions as the player that is used to readjust the position of the teleport.")]
    public GameObject teleportDummy;
    [Tooltip("How fast the player teleports.")]
    public float teleportSpeed = 15f;
    [Tooltip("Time in seconds that the player will stay floating after the teleport ends.")]
    public float floatingTime = 2f;
    public CharacterMovement characterMovement;
    public Animator animator;

    private bool teleporting;
    private bool floating;
    private float currentTimeFloating = 0;
    private bool dummyEnabled;
    private Vector3 tmp;

    void FixedUpdate()
    {
        if (teleporting)
        {
            UpdateTeleport();
        }
        if (floating)
        {
            if (currentTimeFloating >= floatingTime)
            {
                floating = false;
            }
            currentTimeFloating += Time.fixedDeltaTime;
        }
    }

    private void UpdateTeleport()
    {
        if (dummyEnabled)
        {
            // Disable immediately so that it doesn't collide with anything
            DisableDummy();
            dummyEnabled = false;
        }
        // Step by step move player torwards teleport destination
        float step = teleportSpeed * Time.fixedDeltaTime;
        Vector3 currentPosition = transform.position;
        Vector3 toPosition = teleportDummy.transform.position;
        Vector3 nextPosition = Vector3.MoveTowards(currentPosition, toPosition, step);
        transform.position = nextPosition;
        if (nextPosition == toPosition)
        {
            teleporting = false;
            // Once the teleport is finished, let the player float for some time
            floating = true;
            currentTimeFloating = 0;
        }
    }

    public GameObject GetNearestSoul()
    {
        GameObject nearestSoul = null;
        Vector3 currentPosition = transform.position;
        float minDistance = 0;
        List<GameObject> souls = teleportTriggerArea.Souls;
        for (int i = 0; i < souls.Count; i++)
        {
            GameObject soulObject = souls[i];
            Vector3 soulPosition = soulObject.transform.position;
            float xDiff = soulPosition.x - currentPosition.x;
            bool playerIsFacingSoul = xDiff == 0 || Mathf.Sign(xDiff) == characterMovement.FacingDirection;
            float distance = Vector2.Distance(currentPosition, soulPosition);
            // Only if the player is facing the soul
            if (playerIsFacingSoul && (nearestSoul == null || distance < minDistance))
            {
                nearestSoul = soulObject;
                minDistance = distance;
            }
        }
        return nearestSoul;
    }

    public bool DoTeleport()
    {
        // Automatically grab the soul that's closest to the player
        GameObject nearestSoul = GetNearestSoul();
        if (nearestSoul != null)
        {
            teleporting = true;
            // Set the dummy to the position of the soul so that we can use 
            // the position after the collision adjustements have been done
            UpdateDummyPosition(nearestSoul.transform.position);
            animator.SetTrigger("ShadowWalk");
        }
        return teleporting;
    }

    private void UpdateDummyPosition(Vector3 position)
    {
        teleportDummy.SetActive(true);
        teleportDummy.transform.position = position;
        // Move dummy into every direction to resolve collisions and let it readjust position if neccesary
        CharacterController characterController = teleportDummy.GetComponent<CharacterController>();
        tmp.Set(-0.0001f, -0.0001f, -0.0001f);
        characterController.Move(tmp);
        tmp.Set(0.0002f, 0.0002f, 0.0002f);
        characterController.Move(tmp);
        tmp.Set(-0.0001f, -0.0001f, -0.0001f);
        characterController.Move(tmp);
        dummyEnabled = true;
    }

    private void DisableDummy()
    {
        teleportDummy.SetActive(false);
    }

    public bool HasTarget
    {
        get { return teleportTriggerArea.Souls.Count > 0; }
    }

    public bool IsTeleporting
    {
        get { return teleporting; }
    }

    public bool IsFloating
    {
        get { return floating; }
        set { floating = value; }
    }
}
