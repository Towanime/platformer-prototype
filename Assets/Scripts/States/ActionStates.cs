﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionStates {

    // None of the other actions is being performed
    Idle,
    Shooting,
    // Teleporting, cannot be attacked or do any other action
    Teleporting,
    // Floating in the air for some time after a teleport
    Floating,
    // Arm is flying forward/coming back
    Grabbing,
    Jumping,
    // Received damage and knockback
    Damaged,
    Spawning,
    Death
}
