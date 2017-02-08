using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {

	public static bool IsObjectInLayerMask(LayerMask layerMask, GameObject gameObject)
    {
        int objLayerMask = (1 << gameObject.layer);
        return (layerMask.value & objLayerMask) > 0;
    }
}
