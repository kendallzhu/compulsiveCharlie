﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chores : Activity
{
    void Awake()
    {
        name = "Chores";
        descriptionText = "less mess";
        emotionNotes = new EmotionState(0, 0, 1);
        emotionEffect = new EmotionState(1, 3, 1);
        isUnlocked = true;
    }
}
