﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hope : HighEmotionThought
{
    void Awake()
    {
        name = "Whyyy Meee!";
        descriptionText = "Preserve the light";
        isUnlocked = true;
        energyCost = 4;
        maxJumpPower = 1;
        emotionType = EmotionType.despair;
    }
}