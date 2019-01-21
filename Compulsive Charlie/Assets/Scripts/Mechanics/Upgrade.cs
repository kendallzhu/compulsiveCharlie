﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class for upgrade/unlock mechanic in game
public abstract class Upgrade : MonoBehaviour {
    // unique name
    new public string name;
    public string descriptionText;
    public int expCost;
    // TODO: sprite/icon
    
    public bool isUnlocked = false;

    // comb through lists of activities and thoughts and modify them to make upgrade
    public abstract bool MakeUpgrade(Profile profile);

    // criteria to unlock this upgrade during a run
    public abstract bool IsUnlock(RunState runState, Profile profile);
}