using UnityEngine;
using System.Collections;

public class Grab : MonoBehaviour {
    public float distance = 3;
    public float speed = 8f;
    public float speedPenalty = 1.5f;
    // arm needs a rigid body and colission
    public GameObject arm;
    // empty object where the arm should always return
    public GameObject armAnchor;
    public bool isEnabled = true;
    // 
    // did the player press the grab button?
    private bool thrown;
    // is the arm returning to the original position?
    private bool returning;
    // direction the player was facing when the grab started
    private int throwDirection;
    // temporal penalty if theres no collision
    private float returnPenalty;
    // starting throw time
    private float startTime;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float journeyLength;
    private SimplePlayerController controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<SimplePlayerController>();
	}
	
	void FixedUpdate () {
        if (!isEnabled || !thrown) return;

        // check distance manually 
        if (thrown)
        {
            float distCovered = (Time.time - startTime) * (speed - returnPenalty);
            float fracJourney = distCovered / journeyLength;
            arm.transform.position = Vector3.Lerp(initialPosition, targetPosition, fracJourney);
            // used to check if the goal as been reached
            float currentDistance = Vector3.Distance(arm.transform.position, targetPosition);
            // check destination
            if (returning && currentDistance <= 0)//&& arm.transform.position.x <= targetPosition.x)
            {
                // finish grab
                End();
                Debug.Log("Arm returned empty handed! At: " + arm.transform.position.ToString());
            }
            else if(!returning && currentDistance <= 0)// && arm.transform.position.x >= targetPosition.x)
            {
                returning = true;
                returnPenalty = speedPenalty;
                // reset distances and times
                Vector3 tmpInitial = initialPosition;
                initialPosition = targetPosition;
                targetPosition = armAnchor.transform.position;//tmpInitial;
                startTime = Time.time;
                journeyLength = Vector3.Distance(initialPosition, targetPosition);
                Debug.Log("Arm reached max distance! At: " + arm.transform.position.ToString());
            }
        }
	}

    public void Begin(int direction)
    {
        // beging arm thow
        thrown = true;
        throwDirection = direction;
        returnPenalty = 0;
        // data to lerp it in the update
        startTime = Time.time;
        initialPosition = armAnchor.transform.position;//arm.transform.position;
        targetPosition = initialPosition;
        targetPosition.x += distance * direction;
        journeyLength = Vector3.Distance(initialPosition, targetPosition);
        // enable grab trigger!
        arm.GetComponent<BoxCollider2D>().enabled = true;
        Debug.Log("Begin thrown at: " + initialPosition.ToString() + " - with limit: " + targetPosition.ToString());
    }

    private void End()
    {
        // clean up and renable the controller
        thrown = false;
        returning = false;
        startTime = 0;
        controller.IsEnabled = true;
    }

    public void OnCollision(GameObject target)
    {

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
}
