using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util {

    /// <summary>
    /// Checks if the layer of the given GameObject is included in the given layerMask.
    /// </summary>
    /// <param name="layerMask"></param>
    /// <param name="gameObject"></param>
    /// <returns>True if the object is in the layer mask.</returns>
	public static bool IsObjectInLayerMask(LayerMask layerMask, GameObject gameObject)
    {
        int objLayerMask = (1 << gameObject.layer);
        return (layerMask.value & objLayerMask) > 0;
    }
}
