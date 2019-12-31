﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToBed : Activity
{
    void Awake()
    {
        name = "Go To Bed";
        descriptionText = "it's all over";
        heightRating = 0;
        emotionEffect = new EmotionState(2, 0, 2);
        isUnlocked = true;
        song = Luma.song;
        tempoIncrement = .1f;
    }

    public override void Effect(RunState runState)
    {
        runState.done = true;
    }

    // (weighted) availability of activity, given state of run
    public override int CustomAvailability(RunState runState)
    {
        if (runState.timeSteps > 9)
        {
            return 1;
        }
        return 0;
    }

    // make it easier to go to bed the later it is
    public override int HeightRating(RunState runState)
    {
        int lateness = System.Math.Max(0, runState.timeSteps - runState.bedTime);
        return base.HeightRating(runState) - lateness;
    }
}
