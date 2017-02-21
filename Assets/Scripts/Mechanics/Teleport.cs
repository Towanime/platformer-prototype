using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public GameObject shadowWalkEffect;
    [Tooltip("Area that contains the souls nearby.")]
    public TeleportTriggerArea teleportTriggerArea;
    [Tooltip("Object with the same dimentions as the player that is used to readjust the position of the teleport.")]
    public GameObject teleportDummy;
    [Tooltip("How fast the player teleports.")]
    public float teleportSpeed = 40f;
    [Tooltip("Time in seconds that the player will stay floating after the teleport ends.")]
    public float floatingTime = 1f;
    public AimingDirectionResolver aimingDirectionResolver;
    public Animator animator;
    [Tooltip("Only target souls found in the direction the player is looking at.")]
    public bool onlyTargetForward = true;
    [Tooltip("Width of the soul when it's being targetted.")]
    public float soulOutlineWidth = 3f;
    [Tooltip("Minimum distance between the player and the soul that is needed for it to be targetable.")]
    public float minDistanceToDetect = 0.2f;
    private float currentTimeFloating = 0;
    private bool dummyEnabled;
    private Vector3 tmp;
    private GameObject nearestSoul;

    [Tooltip("Time before the particles are turned off after the teleport is finished.")]
    public float timeBeforeStoppingParticles = 1f;
    [Tooltip("Time before the trails are turned off after the teleport is finished.")]
    public float timeBeforeHiddingTrails = 0.15f;
    private ParticleSystem particleSystem;
    private TrailRenderer[] trailRenderers;
    /// <summary>
    /// Exact time when the teleport ended
    /// </summary>
    private float timeWhenTeleportEnded;
    private bool timerStartedToHideParticles;
    private bool timerStartedToHideTrails;

    void Start()
    {
        particleSystem = shadowWalkEffect.GetComponentInChildren<ParticleSystem>();
        trailRenderers = shadowWalkEffect.GetComponentsInChildren<TrailRenderer>();
    }

    void Update()
    {
        UpdateParticleSystem();
        UpdateTrailRenderer();
    }

    void FixedUpdate()
    {
        UpdateNearestSoul();
    }

    void UpdateParticleSystem()
    {
        if (timerStartedToHideParticles)
        {
            if (Time.time - timeWhenTeleportEnded >= timeBeforeStoppingParticles)
            {
                TurnOffParticles();
            }
        }
    }

    void UpdateTrailRenderer()
    {
        if (timerStartedToHideTrails)
        {
            if (Time.time - timeWhenTeleportEnded >= timeBeforeHiddingTrails)
            {
                TurnOffTrails();
            }
        }
    }

    void TurnOffParticles()
    {
        particleSystem.Stop();
        timerStartedToHideParticles = false;
    }

    void TurnOffTrails()
    {
        foreach (TrailRenderer trailRenderer in trailRenderers)
        {
            trailRenderer.enabled = false;
        }
        timerStartedToHideTrails = false;
    }

    public void StopShadowWalkEffect(bool withTimer)
    {
        if (withTimer)
        {
            timeWhenTeleportEnded = Time.time;
            timerStartedToHideParticles = true;
            timerStartedToHideTrails = true;
        } else
        {
            TurnOffParticles();
            TurnOffTrails();
        }
    }

    private void UpdateNearestSoul()
    {
        GameObject oldNearestSoul = nearestSoul;
        nearestSoul = GetNearestSoul();
        // Change the outline of the nearest soul
        if (oldNearestSoul != nearestSoul)
        {
            // Make it invisible for the old nearest soul
            if (oldNearestSoul != null)
            {
                Renderer renderer = oldNearestSoul.GetComponent<Renderer>();
                renderer.material.SetFloat("_Outline", 0);
            }
            // Make it visible for the new one
            if (nearestSoul != null)
            {
                Renderer renderer = nearestSoul.GetComponent<Renderer>();
                renderer.material.SetFloat("_Outline", soulOutlineWidth);
            }
        }
    }

    public void BeginFloating()
    {
        currentTimeFloating = 0;
    }

    public bool UpdateFloating()
    {
        currentTimeFloating += Time.fixedDeltaTime;
        return currentTimeFloating >= floatingTime;
    }

    public bool BeginTeleport()
    {
        // Automatically grab the soul that's closest to the player
        if (nearestSoul != null)
        {
            // Set the dummy to the position of the soul so that we can use 
            // the position after the collision adjustements have been done
            UpdateDummyPosition(nearestSoul.transform.position);
            return true;
        }
        return false;
    }

    public void StartShadowWalkEffect()
    {
        shadowWalkEffect.transform.rotation = Quaternion.LookRotation(nearestSoul.transform.position - transform.position);
        shadowWalkEffect.SetActive(true);
        particleSystem.Play();
        foreach (TrailRenderer trailRenderer in trailRenderers)
        {
            trailRenderer.enabled = true;
        }
        timerStartedToHideParticles = false;
        timerStartedToHideTrails = false;
        animator.SetTrigger("ShadowWalk");
    }

    public bool UpdateTeleport()
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
            return true;
        }
        return false;
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
            bool playerIsFacingSoul = xDiff == 0 || Mathf.Sign(xDiff) == aimingDirectionResolver.FacingDirection;
            float distance = Vector2.Distance(currentPosition, soulPosition);
            bool inRange = distance >= minDistanceToDetect;
            // Only if the player is facing the soul
            if ((!onlyTargetForward || playerIsFacingSoul) && (nearestSoul == null || distance < minDistance) && inRange)
            {
                nearestSoul = soulObject;
                minDistance = distance;
            }
        }
        return nearestSoul;
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
        get { return nearestSoul != null; }
    }

    public bool IsTeleporting
    {
        get { return true; }
    }

    public bool IsFloating
    {
        get { return true; }
        set { bool a = value; }
    }
}
