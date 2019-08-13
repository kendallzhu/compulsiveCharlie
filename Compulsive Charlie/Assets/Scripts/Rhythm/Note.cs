﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Note : MonoBehaviour
{
    RunManager runManager;
    RhythmManager rhythmManager;
    Transform hitArea;

    public bool isResolved = false;
    float spawnTime;
    public float arrivalTime;
    Vector2 startingOffset;
    public EmotionType type;
    // "animator" prefabs
    public GameObject hitPrefab;
    public GameObject missPrefab;
    public GameObject deflectPrefab;

    void Awake()
    {
        runManager = FindObjectOfType<RunManager>();
        rhythmManager = FindObjectOfType<RhythmManager>();
        hitArea = GameObject.FindWithTag("HitArea").transform;
    }

    public void Initialize(float trueSpawnTime, AudioClip soundClip)
    {
        spawnTime = trueSpawnTime;  
        arrivalTime = trueSpawnTime + RhythmManager.travelTime;
        startingOffset = transform.position - hitArea.position;
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = soundClip;
        GameObject arrow = transform.Find("Arrow").gameObject;
        if (arrow)
        {
            arrow.SetActive(false);
        }
    }

    // hack to generate unique float for each note in a group, so their colors don't blend weirdly when they overlap
    private float GetZPosition(EmotionType type)
    {
        float offset = startingOffset.y * .0001f;
        if (type == EmotionType.frustration)
        {
            return 0.01f + offset;
        } else if (type == EmotionType.anxiety)
        {
            return 0.01f + offset;
        } else if (type == EmotionType.despair)
        {
            return 0.02f + offset;
        }
        Debug.Assert(type == EmotionType.None);
        return 0.03f + offset;
    }

    void Update()
    {
        // move note to proper position
        float scaleFactor = (arrivalTime - rhythmManager.time) / RhythmManager.travelTime;
        scaleFactor = Math.Max(0, scaleFactor);
        Vector3 newPos = (Vector2)hitArea.position + startingOffset * scaleFactor;
        // use z position for ordering to make overlap colors more normal
        transform.position = new Vector3(newPos.x, newPos.y, GetZPosition(type));
    }

    public void OnDeflect()
    {
        isResolved = true;
        Destroy(gameObject);
        GameObject deflect = Instantiate(deflectPrefab, transform.position, Quaternion.identity, transform.parent);
        deflect.GetComponent<MoveFadeOut>().SetDirection(new Vector2(-startingOffset.x, startingOffset.y));
    }

    // functions for miss/hit effects
    public void OnMiss(RunState runState)
    {
        isResolved = true;
        runState.BreakCombo();
        MissEffect(runState);
        Destroy(gameObject);
        Instantiate(missPrefab, transform.position, Quaternion.identity, hitArea);
        rhythmManager.player.GetComponent<Animator>().SetTrigger("activityFail");
    }

    // (Delay so that sound occurs when note would have arrived)
    public IEnumerator HitAfterDelay(float delay, RunState runState)
    {
        isResolved = true;
        transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(delay);
        runState.IncreaseCombo();
        HitEffect(runState);
        Instantiate(hitPrefab, transform.position, Quaternion.identity, hitArea);
        rhythmManager.player.GetComponent<Animator>().ResetTrigger("activityFail");
        // play audio and destroy when done
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();
        Destroy(gameObject, audioSource.clip.length);
    }

    public void OnHit(float time, RunState runState)
    {
        StartCoroutine(HitAfterDelay(arrivalTime - time + RhythmManager.hitWindowLate, runState));
    }

    public IEnumerator AutoHitAfterDelay(float delay)
    {
        isResolved = true;
        // dark out note and move it back
        transform.localScale = new Vector3(.5f, .5f, 0);
        transform.Translate(new Vector3(0, 0, -1f));
        yield return new WaitForSeconds(delay);
        transform.localScale = new Vector3(0f, 0f, 0);
        // Instantiate(hitPrefab, transform.position, Quaternion.identity, hitArea);
        // play audio and destroy when done
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();
        Destroy(gameObject, audioSource.clip.length);
    }

    public void OnAutoHit(float time)
    {
        // seperate AutoHitAfterDelay function with less effects?
        StartCoroutine(AutoHitAfterDelay(arrivalTime - time + RhythmManager.hitWindowLate));
    }

    public virtual void HitEffect(RunState runState)
    {
        return;
    }

    public virtual void MissEffect(RunState runState)
    {
        return;
    }
}
