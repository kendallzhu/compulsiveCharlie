﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImScrewed : Thought
{
    void Awake()
    {
        name = "I'm Screwed";
        descriptionText = "It's falling apart";
        isUnlocked = true;
        energyCost = 0;
        jumpPower = 0;
        emotionType = EmotionType.anxiety;
    }

    // whether this activity is available, given state of run
    public override int CustomAvailability(RunState runState)
    {
        int value = runState.emotions.anxiety;
        if (runState.emotions.GetDominantEmotion() == emotionType && value >= 5 && value <= 15)
        {
            return 1;
        }
        return 0;
    }
}