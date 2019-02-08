﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : Activity
{
    void Awake()
    {
        name = "Walk";
        descriptionText = "go on a walk";
        emotionNotes = new EmotionState(1, 1, 1);
        isUnlocked = true;
    }
}
