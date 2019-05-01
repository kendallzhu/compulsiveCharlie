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
        emotionNotes = new EmotionState(0, 1, 0);
        emotionEffect = new EmotionState(3, 3, 0);
        rhythmPattern = new List<int> { 2, 4, 6, 8 };
        isUnlocked = true;
    }

    // (weighted) availability of activity, given state of run
    public override int CustomAvailability(RunState runState)
    {
        // only when scheduled
        return 0;
    }
}
