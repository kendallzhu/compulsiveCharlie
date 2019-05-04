﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNothing : Activity
{
    void Awake()
    {
        name = "Do Nothing";
        descriptionText = "wait - actually nothing?";
        heightRating = 0;
        emotionNotes = new EmotionState(1, 0, 0);
        emotionEffect = new EmotionState(0, 0, 0);
        rhythmPattern = new List<int> { 4, 5, 6, 7 };
        isUnlocked = true;
    }

    // (weighted) availability of activity, given state of run
    public override int CustomAvailability(RunState runState)
    {
        // only used if all other default activities are not available (see runManager)
        return 0;
    }

    // height of associated platform if it comes after given run state
    public override int HeightRating(RunState runState)
    {
        // special - always be the default when available
        return runState.emotions.GetRaiseAmount() + defaultPlatformHeightDiff;
    }
}
