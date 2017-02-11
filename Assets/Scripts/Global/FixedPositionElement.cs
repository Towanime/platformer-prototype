using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPositionElement : MonoBehaviour {
	
	void Update () {
        // For now just set Z to 0, can be improved if we want something more customizable
        Vector3 position = transform.position;
        position.z = 0;
        transform.position = position;
    }
}
