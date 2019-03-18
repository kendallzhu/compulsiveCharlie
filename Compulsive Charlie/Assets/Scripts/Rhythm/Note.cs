﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    RhythmManager rhythmManager;
    Transform hitArea;

    float spawnTime;
    public float arrivalTime;
    float travelDistance;

    void Awake()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
        hitArea = GameObject.FindWithTag("HitArea").transform;
    }

    public void Initialize(float trueSpawnTime)
    {
        spawnTime = trueSpawnTime;
        arrivalTime = trueSpawnTime + RhythmManager.travelTime;
        travelDistance = transform.position.x - hitArea.position.x;
    }

    void Update()
    {
        // move note to proper position
        float newX = hitArea.position.x + travelDistance * (arrivalTime - rhythmManager.time) / RhythmManager.travelTime;
        newX = System.Math.Max(hitArea.position.x, newX);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    // TODO: functions to display show miss/hit, also note types (inheritance?)
    public void OnMiss(RunState runState)
    {
        Destroy(gameObject);
    }

    public void OnHit(RunState runState)
    {
        runState.IncreaseEnergy(1);
        Destroy(gameObject);
    }
}
