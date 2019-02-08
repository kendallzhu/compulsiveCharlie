﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eating : Activity
{
    void Awake()
    {
        name = "Eating";
        descriptionText = "it comes naturally";
        isUnlocked = true;
    }

    // (weighted) availability of activity, given state of run
    public override int CustomAvailability(RunState runState)
    {
        if (runState.emotions.GetDominantEmotion() == "despair")
        {
            return 1;
        }
        return 0;
    }
}
