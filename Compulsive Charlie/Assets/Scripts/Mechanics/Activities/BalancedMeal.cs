﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalancedMeal : Activity
{
    void Awake()
    {
        name = "Balanced Meal";
        descriptionText = "fruits and veggies";
        heightRating = 3;
        emotionEffect = new EmotionState(6, 6, 0);
        isUnlocked = true;
    }

    // (weighted) availability of activity, given state of run
    public override int CustomAvailability(RunState runState)
    {
        // only when scheduled
        return 0;
    }
}
