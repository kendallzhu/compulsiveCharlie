﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Binge : Activity
{
    void Awake()
    {
        name = "Binge";
        descriptionText = "fill the hole with food";
        heightRating = -1;
        song = MumenRider.song;
        tempoIncrement = .2f;
        emotionEffect = new EmotionState(0, 0, 8);
        suppressedEmotions.Add(EmotionType.despair);
        suppressedEmotions.Add(EmotionType.anxiety);
        isUnlocked = true;
    }

    // (weighted) availability of activity, given state of run
    public override int CustomAvailability(RunState runState)
    {
        Activity balancedMeal = Object.FindObjectOfType<BalancedMeal>();
        // offer whenever it's time for a meal
        if (balancedMeal.Availability(runState) > 0)
        {
            return 3;
        }
        // or if meal was skipped
        int timeSinceEat = System.Math.Min(runState.TimeSinceLast(this), runState.TimeSinceLast(balancedMeal));
        int hunger = System.Math.Max(0, timeSinceEat - 3);
        return hunger;
    }
}
