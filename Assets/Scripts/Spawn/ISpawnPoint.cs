using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnPoint {

    /// <summary>
    /// Initiate the condition checks and timers for a spawn point before the enemy can be created.
    /// </summary>
    /// <returns></returns>
    bool Spawn();

}
