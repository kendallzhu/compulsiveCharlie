﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImScrewed : LowEmotionThought
{
    void Awake()
    {
        name = "I'm Screwed";
        descriptionText = "It's falling apart";
        isUnlocked = true;
        energyCost = 0;
        maxJumpPower = 0;
        emotionType = EmotionType.anxiety;
    }
}