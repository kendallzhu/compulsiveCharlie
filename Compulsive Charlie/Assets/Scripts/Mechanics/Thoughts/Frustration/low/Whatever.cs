﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whatever : Thought
{
    void Awake()
    {
        name = "Whatever";
        descriptionText = "Not gonna let it get to me";
        isUnlocked = true;
        energyLevel = 0;
        jumpPower = 2;
        invisibleEmotions = new List<EmotionType> { EmotionType.frustration };
    }

    // whether this activity is available, given state of run
    public override int CustomAvailability(RunState runState)
    {
        int value = runState.emotions.frustration;
        if (value >= 5 && value <= 15)
        {
            return 1;
        }
        return 0;
    }
}