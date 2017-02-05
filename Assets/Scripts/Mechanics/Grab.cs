using UnityEngine;
using System.Collections;

public class Grab : MonoBehaviour
{
    [Tooltip("Units from initial point to max distance.")]
    public float distance = 3;
    [Tooltip("Speed for the arm.")]
    public float speed = 8f;
    public float speedPenalty = 1.5f;
    [Tooltip("Cooldown before attepting another grab or crunch.")]
    // time before being able to grab again
    public float grabCooldown = 0.1f;
    // arm needs a rigid body and colission
    [Tooltip("Extra arm game object, need a rigid body, collission and be on the Grab layer.")]
    public GameObject arm;
    // empty object inside the player where the arm should always return
    [Tooltip("Empty object inside the player where the arm should always return.")]
    public GameObject armAnchor;
    // empty object used to keep the target relative to the player in case it moves
    [Tooltip("Empty object inside the player used for calculations but it'll be likely removed later!.")]
    public GameObject armTarget;
    [Tooltip("Renderer of the rooted arm in the model, it'll be disabled when doing a grab")]
    public Renderer originalArmRenderer;
    public bool isEnabled = true;
    // 
    // did the player press the grab button?
    private bool thrown;
    // is the arm returning to the original position?
    private bool returning;
    // temporal penalty if theres no collision
    private float returnPenalty;
    // starting throw time
    private float startTime;
    private float journeyLength;
    private SimplePlayerController controller;
    private CharacterMovement characterMovement;
    private Animator animator;
    // grabbed enemy
    private GameObject grabbedEnemy;
    // cooldown vars
    private float currentCooldown;
    private bool wait;
    // true when the arm is going and coming back
    private bool isRunning;
    // animation ended?
    private bool animationEnded;

    // Use this for initialization
    void Start () {
        controller = GetComponent<SimplePlayerController>();
        characterMovement = GetComponent<CharacterMovement>();
        animator = GetComponent<Animator>();
	}

    void Update()
    {
        if (wait)
        {
            currentCooldown += Time.deltaTime;
            // turn off wait if the time is up
            if (currentCooldown >= grabCooldown)
            {
                wait = false;
            }
        }
    }
	
	void FixedUpdate () {
        if (!isEnabled) return;
        // distance to traverse between player and target
        journeyLength = Vector3.Distance(armAnchor.transform.position, armTarget.transform.position);
        float distCovered = (Time.time - startTime) * (speed - returnPenalty);
        float fracJourney = distCovered / journeyLength;

        // check distance manually 
        if (thrown)
        {
            // update position of the arm
            arm.transform.position = Vector3.Lerp(armAnchor.transform.position, armTarget.transform.position, fracJourney);

            // got to the target?
            if(fracJourney >= 1)
            {
                Comeback(true);
                Debug.Log("Arm reached max distance! At: " + arm.transform.position.ToString());
            }
        } else if (returning)
        {
            arm.transform.position = Vector3.Lerp(armTarget.transform.position, armAnchor.transform.position, fracJourney);
            // check destination
            if (fracJourney >= 1)
            {
                // not traveling anymore
                isRunning = false;
                // finish grab
                End();
                Debug.Log("Arm returned empty handed! At: " + arm.transform.position.ToString());
            }
        }
	}

    public bool Begin()
    {
        if (!CanAct()) return false;
        // dont do anything until the transition is complete
        if (!animationEnded)
        {
            // init the state change
            animator.SetBool("IsGrabbing", true);
        }        
        return true;
    }
    
    /// <summary>
    /// What to do after the animation transition is done and the arm should start traveling.
    /// </summary>
    public void Throw()
    {
        // beging arm thow
        thrown = true;
        animationEnded = true;
        grabbedEnemy = null;
        returnPenalty = 0;
        // bool so other components know when it's working
        isRunning = true;
        // data to lerp it in the update
        startTime = Time.time;

        // enable throw arm and hide the animated one
        originalArmRenderer.enabled = false;
        arm.SetActive(true);

        // setup target for the arm
        Vector3 targetPosition = armAnchor.transform.position;
        targetPosition.x += distance * characterMovement.FacingDirection;
        armTarget.transform.position = targetPosition;

        // enable grab trigger!
        arm.GetComponent<Collider>().enabled = true;
        Debug.Log("Begin thrown at: " + armAnchor.transform.position.ToString() + " - with limit: " + targetPosition.ToString());
    }


    /// <summary>
    /// What it does when the arm comes back to the player.
    /// </summary>
    /// <param name="penalty"></param>
    private void Comeback(bool penalty)
    {
        // start returning with delay
        returning = true;
        thrown = false;
        returnPenalty = penalty ? speedPenalty : 0;
        startTime = Time.time;
        arm.GetComponent<Collider>().enabled = false;
    }

    private void End()
    {
        // clean up and renable the controller
        thrown = false;
        returning = false;
        animationEnded = false;
        startTime = 0;
        controller.IsEnabled = true;
        // begin cooldown
        wait = true;
        currentCooldown = 0;
        animator.SetBool("IsGrabbing", false);
        arm.SetActive(false);
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
    /// Checks cooldown to see if the player can use the ability.
    /// </summary>
    /// <returns></returns>
    public bool CanAct()
    {
        return !isRunning && currentCooldown >= grabCooldown || !wait;
    }

    public void OnCollision(Collider collision)
    {
        // update arm target position to the contact point!
        armTarget.transform.position = arm.transform.position;

        GameObject target = collision.gameObject;

        GrababbleEntity canGrab = target.GetComponent<GrababbleEntity>();
        // leave it done its grab behavior and attach the body if returns true
        if (canGrab && canGrab.OnGrab())
        {
            // make it child of the arm anchor or arm object??
            target.transform.parent = arm.transform;

            Rigidbody objectRigidBody = target.GetComponent<Rigidbody>();
            objectRigidBody.isKinematic = true;
            // change position of the grabbed object
            target.transform.position = armAnchor.transform.position;
            target.transform.localPosition = new Vector3(1.5f, 0.5f, 0);
            // store the grabbed enemy
            grabbedEnemy = target;
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
            return this.isRunning;
        }
    }
}
