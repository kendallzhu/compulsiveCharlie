﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent class for thought mechanic in game
public abstract class Thought : MonoBehaviour {
    // unique name
    new public string name;

    // changeable parameters
    public bool isUnlocked = false;
    Dictionary<string, int[]> availabilityThresholds;

    // whether this thought is available, given state of run
    // Don't use activity history (to keep modular)
    public abstract bool IsAvailable(RunState runState);

    // how this thought modifies given state of run
    public abstract void Effect(RunState runState);
}