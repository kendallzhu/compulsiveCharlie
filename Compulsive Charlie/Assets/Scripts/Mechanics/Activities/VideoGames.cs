﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoGames : Activity
{
    void Awake()
    {
        name = "Video Games";
        descriptionText = "...";
        heightRating = -1;
        emotionNotes = new EmotionState(0, 1, 0);
        emotionEffect = new EmotionState(0, 4, 0);
        rhythmPattern = new List<int> { 1, 3, 7 };
        isUnlocked = true;
    }

    // (weighted) availability of activity, given state of run
    public override int CustomAvailability(RunState runState)
    {
        if (runState.emotions.GetDominantEmotion() == EmotionType.frustration)
        {
            return 1;
        }
        return 0;
    }

    public override void Effect(RunState runState)
    {
        runState.emotions.Add(EmotionType.despair, 1);
        return;
    }
}
