﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : Activity
{
    void Awake()
    {
        name = "Walk";
        descriptionText = "go on a walk";
        heightRating = 0;
        emotionEffect = new EmotionState(0, 0, 0);
        isUnlocked = true;
        song = Luma.song;
        tempoIncrement = .11f;
    }
}
