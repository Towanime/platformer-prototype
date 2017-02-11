using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableEntity : MonoBehaviour {
    /// <summary>
    /// Activated on the first time the player grabs a game object, override to implement different behavior.
    /// By default the element with this grabbable entity should be destroyed when thrown or crushed.
    /// </summary>
    public bool alreadyGrabbed;

    /// <summary>
    /// Checks conditions and prepares a body to be able attached to the arm. Override this method for different enemy grab conditions.
    /// </summary>
    /// <returns>Returns a gameobject ready to be attached or null if there's a condition to meet before grabbing an enemy.</returns>
	public virtual GameObject OnGrab()
    {
        if (alreadyGrabbed) return null;
        Rigidbody objectRigidBody = GetComponent<Rigidbody>();
        objectRigidBody.isKinematic = true;
        alreadyGrabbed = true;
        return gameObject;
    }
    
}
