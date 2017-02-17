using UnityEngine;
using System.Collections;

public class Grab : MonoBehaviour
{
    private enum ArmState { Default, WaitingAnimation, Thrown, Returning };

    [Tooltip("Units from initial point to max distance.")]
    public float distance = 3;
    [Tooltip("Speed for the arm.")]
    public float speed = 12f;
    public float speedPenalty = 1.5f;
    [Tooltip("Cooldown before attepting another grab or crunch.")]
    // time before being able to grab again
    public float grabCooldown = 0.1f;
    [Tooltip("Force applied when throwing an enemy.")]
    public float throwSpeed = 12f;
    [Tooltip("Time that the enemy will spend traveling unitl reaching 0 speed.")]
    public float throwEnemyTravelTime = 2f;
    [Tooltip("Time that the enemy will spend in the air before dissapearing. Usually the same as throwEnemyTravelTime")]
    public float throwDeathTime = 2f;
    [Tooltip("Speed curve of the enemy throw.")]
    public AnimationCurve throwSpeedCurve;
    // arm needs a rigid body and colission
    [Tooltip("Extra arm game object, need a rigid body, collission and be on the Grab layer.")]
    public GameObject arm;
    // empty object inside the player where the arm should always return
    [Tooltip("Empty object inside the player where the arm should always return.")]
    public GameObject armAnchor;
    // empty object used to keep the target relative to the player in case it moves
    [Tooltip("Empty object inside the player used for calculations but it'll be likely removed later!.")]
    public GameObject armTarget;
    [Tooltip("Position that will be applied to a grabbed object relative to the position of the hand.")]
    public Vector3 grabPositionOffset;
    [Tooltip("Rotation that will be added to a grabbed object relative to its original rotation.")]
    public Vector3 grabRotationOffset;
    [Tooltip("Renderer of the rooted arm in the model, it'll be disabled when doing a grab")]
    public Renderer originalArmRenderer;
    public AimingDirectionResolver aimingDirectionResolver;
    public Animator animator;
    public bool isEnabled = true;
    // has been thrown? is it returning?
    private ArmState currentArtState;
    // temporal penalty if theres no collision
    private float returnPenalty;
    // starting throw time
    private float startTime;
    private float journeyLength;
    // renderer for the hidden arm
    private Renderer armRenderer;
    // grabbed enemy
    private GameObject grabbedEnemy;
    // cooldown vars
    private float currentCooldown;
    private bool waitCooldown;
    // animation ended?
    private bool animationEnded;

    // Use this for initialization
    void Start()
    {
        armRenderer = arm.GetComponent<Renderer>();
    }

    void Update()
    {
        if (waitCooldown)
        {
            currentCooldown += Time.deltaTime;
            // turn off wait if the time is up
            if (currentCooldown >= grabCooldown)
            {
                waitCooldown = false;
            }
        }
    }
	
	void FixedUpdate () {
        if (!isEnabled) return;

        // distance to traverse between player and target if the arm is being thrown or is returning
        //if (currentArtState != ArmState.Default) {
            journeyLength = Vector3.Distance(armAnchor.transform.position, armTarget.transform.position);
            float distCovered = (Time.time - startTime) * (speed - returnPenalty);
            float fracJourney = distCovered / journeyLength;
        //}

        switch (currentArtState)
        {
            case ArmState.Thrown:
                // update position of the arm
                arm.transform.position = Vector3.Lerp(armAnchor.transform.position, armTarget.transform.position, fracJourney);
                // got to the target?
                if (fracJourney >= 1)
                {
                    Comeback(true);
                    Debug.Log("Arm reached max distance! At: " + arm.transform.position.ToString());
                }
                break;

            case ArmState.Returning:
                arm.transform.position = Vector3.Lerp(armTarget.transform.position, armAnchor.transform.position, fracJourney);
                // check destination
                if (fracJourney >= 1)
                {
                    // finish grab
                    End();
                    Debug.Log("Arm returned empty handed! At: " + arm.transform.position.ToString());
                }
                break;
        }
	}

    public bool Begin()
    {
        if (!CanAct() || !isEnabled) return false;
        // dont do anything until the transition is complete
        if (!animationEnded)
        {
            currentArtState = ArmState.WaitingAnimation;
            // init the state change
            animator.SetTrigger("ThrowArm");
            //animator.SetBool("IsGrabbing", true);
        }        
        return true;
    }
    
    /// <summary>
    /// What to do after the animation transition is done and the arm should start traveling.
    /// </summary>
    public void ArmThrow()
    {
        // beging arm thow
        currentArtState = ArmState.Thrown;
        animationEnded = true;
        grabbedEnemy = null;
        returnPenalty = 0;
        // data to lerp it in the update
        startTime = Time.time; animator.SetBool("IsGrabbing", true);

        // enable throw arm and hide the animated one
        originalArmRenderer.enabled = false;
        armRenderer.enabled = true;

        // setup target for the arm
        Vector3 targetPosition = armAnchor.transform.position;
        targetPosition.x += distance * aimingDirectionResolver.FacingDirection;
        armTarget.transform.position = targetPosition;

        // enable grab trigger!
        arm.GetComponent<Collider>().enabled = true;
        Debug.Log("Begin thrown at: " + armAnchor.transform.position.ToString() + " - with limit: " + targetPosition.ToString());
    }


    /// <summary>
    /// What it does when the arm comes back to the player.
    /// </summary>
    /// <param name="penalty">Apply speed penalty if nothing was grabbed.</param>
    private void Comeback(bool penalty)
    {
        // start returning with delay
        currentArtState = ArmState.Returning;
        returnPenalty = penalty ? speedPenalty : 0;
        startTime = Time.time;
        arm.GetComponent<Collider>().enabled = false;
    }

    private void End()
    {
        currentArtState = ArmState.Default;
        // clean up and renable the controller
        animationEnded = false;
        startTime = 0;
        // begin cooldown
        waitCooldown = true;
        currentCooldown = 0;
        animator.SetBool("IsGrabbing", grabbedEnemy != null);
        armRenderer.enabled = false;
        originalArmRenderer.enabled = true;
    }

    /// <summary>
    /// Destroys grabbed object.
    /// </summary>
    public bool Crush()
    {
        if (grabbedEnemy == null) return false;
        // kill the damn thing
        grabbedEnemy.SendMessage("OnDeath");
        // destroy object
        Destroy(grabbedEnemy);
        grabbedEnemy = null;
        End();
        return true;
    }

    /// <summary>
    /// Throws an enemy in the direction that the player is aiming.
    /// </summary>
    public bool ThrowEnemy(Vector2 aimingDirection, Vector3 origin)
    {
        if (grabbedEnemy == null) return false;
        ThrowBehavior grababbleEntity = grabbedEnemy.GetComponent<ThrowBehavior>();
        // Reset Z position to 0 before throwing?
        grababbleEntity.transform.position = origin;
        grababbleEntity.BeginThrow(aimingDirection, throwSpeed, throwEnemyTravelTime, throwDeathTime, throwSpeedCurve);
        grabbedEnemy.transform.parent = null;
        grabbedEnemy = null;
        End();
        return true;
    }

    /// <summary>
    /// Checks cooldown to see if the player can use the ability.
    /// </summary>
    /// <returns></returns>
    public bool CanAct()
    {
        return currentArtState == ArmState.Default || !waitCooldown;
    }

    /// <summary>
    /// Used by the trigger on the arm to let the component know a collision has happened and should evaluate the grab.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollision(Collider collision)
    {
        // update arm target position to the contact point!
        armTarget.transform.position = arm.transform.position;

        GameObject target = collision.gameObject;

        GrabbableEntity entity = collision.gameObject.GetComponent<GrabbableEntity>();
        GameObject toAttach;
        // leave it done its grab behavior and attach the body if returns something
        if (entity && (toAttach = entity.OnGrab()) != null)
        {
            // make it child of the arm anchor or arm object??
            toAttach.transform.parent = arm.transform;

            // change position of the grabbed object
            toAttach.transform.position = armAnchor.transform.position;
            toAttach.transform.localPosition = grabPositionOffset;
            toAttach.transform.rotation = toAttach.transform.rotation * Quaternion.Euler(grabRotationOffset);
            // store the grabbed enemy
            grabbedEnemy = toAttach;
            // comeback with the target witout penalty
            Comeback(false);
        }else
        {
            // if not grababble comeback with penalty?
            Comeback(true);
        }
    }

    /// <summary>
    /// Used to check if the player has a grabbed enemy
    /// </summary>
    public bool IsHolding
    {
        get { return grabbedEnemy != null; }
    }

    public bool IsEnabled
    {
        set
        {
            this.isEnabled = value;
        }
        get
        {
            return this.isEnabled;
        }
    }

    public bool IsRunning
    {
        get
        {
            return currentArtState != ArmState.Default;//this.isRunning;
        }
    }
}
