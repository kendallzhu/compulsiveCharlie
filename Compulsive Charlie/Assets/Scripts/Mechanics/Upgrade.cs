﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class for upgrade/unlock mechanic in game
public abstract class Upgrade : MonoBehaviour {
    // unique name
    new public string name;
    public string descriptionText;
    // energy, emotion, thought, or action
    public string category;
    // TODO: sprite/icon

    // comb through lists of activities and thoughts and modify them to make upgrade
    public abstract void Activate(Profile profile);

    // criteria to unlock this upgrade during a run
    public abstract bool IsUnlock(RunState runState, Profile profile);
}
