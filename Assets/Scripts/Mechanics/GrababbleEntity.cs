using UnityEngine;
using System.Collections;

public class GrababbleEntity : MonoBehaviour {

    /// <summary>
    /// Checks conditions and prepares a body to be able attached to the arm.
    /// </summary>
    /// <returns>Returns true if the grab is successful or false if there's a condition to meet before grabbing an enemy.</returns>
	public virtual bool OnGrab()
    {
        return true;
    }
}
