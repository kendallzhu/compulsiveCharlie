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
        energyCost = 3;
        jumpPower = 1;
        emotionType = EmotionType.frustration;
    }

    // whether this activity is available, given state of run
    public override int CustomAvailability(RunState runState)
    {
        int value = runState.emotions.frustration;
        if (runState.emotions.GetDominantEmotion() == emotionType && value >= 5 && value <= 15)
        {
            return 1;
        }
        return 0;
    }
}