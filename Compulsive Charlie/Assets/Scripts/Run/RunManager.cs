﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

// Script managing all the state and gameplay of a single run
public class RunManager : MonoBehaviour
{
    public const int minPlatformHeightDiff = 3;

    public GameManager gameManager;
    public RunState runState;
    public PlayerController player;
    public RhythmManager rhythmManager;
    public new CameraController camera;
    public ThoughtMenu thoughtMenu;
    public TutorialManager tutorialManager;

    // prefabs
    public GameObject platformPrefab;

    // Initialization
    void Awake()
    {
        // get reference to gameManager
        gameManager = Object.FindObjectOfType<GameManager>();
        // if no gameManager, load preload scene first
        if (gameManager == null)
        {
            SceneManager.LoadScene(0);
            return;
        }
        // get reference to playerController and rhythmManager
        player = Object.FindObjectOfType<PlayerController>();
        rhythmManager = Object.FindObjectOfType<RhythmManager>();
        // get initial runState based on profile
        runState = new RunState(
            gameManager.profile.initialEnergy,
            gameManager.profile.energyCap,
            new EmotionState(gameManager.profile.initialEmotions)
        );
        Time.timeScale = 1;
    }

    private void Update()
    {
        // cheatcode to skip to recap
        if (Input.GetKey("1") && Input.GetKey("2") && Input.GetKey("3"))
        {
            gameManager.EndRun(runState);
        }
        // cheatcode to deactivate tutorials
        if (Input.GetKey("n") && Input.GetKey("o"))
        {
            tutorialManager.Skip();
            gameManager.showTutorial = false;
        }
        // cheatcodes to add combo/energy and reduce emotion
        if (Input.GetKeyDown("0"))
        {
            runState.IncreaseCombo();
        }
        if (Input.GetKeyDown("9"))
        {
            runState.emotions.Equilibrate();
        }
    }

    // for when the player arrives on next activity, called via trigger in ActivityPlatform
    public void AdvanceTimeStep(ActivityPlatform newActivityPlatform)
    {
        if (newActivityPlatform != null)
        {
            // increment timeSteps
            runState.timeSteps += 1;

            // update activity history
            runState.activityHistory.Add(newActivityPlatform);
            runState.height = newActivityPlatform.y;

            // make first activity "sleep in"
            if (newActivityPlatform.activity == null)
            {
                Debug.Assert(runState.activityHistory.Count == 1);
                newActivityPlatform.activity = Object.FindObjectOfType<SleepIn>();
                runState.timeSteps = 0;
            } else
            {
                // clear out other spawnedPlatforms
                runState.ClearSpawned(newActivityPlatform);
                // trigger activity special effect
                newActivityPlatform.activity.Effect(runState);
                // increase schedule points if applicable
                if (newActivityPlatform.activity == gameManager.profile.GetSchedule(runState.timeSteps))
                {
                    runState.schedulePoints++;
                }
            }
            // start new platform spawning rhythm notes
            rhythmManager.StartRhythm(newActivityPlatform.activity);
            // start activity animation
            player.GetComponent<Animator>().SetInteger("activityHash", Animator.StringToHash(newActivityPlatform.activity.name));
            player.GetComponent<Animator>().SetTrigger("startActivity");
        }

        // return zoom to normal
        camera.ZoomNormal();
    }

    // for when the player enters the jump Pad
    public void EnterJumpPad(ActivityPlatform activityPlatform)
    {
        // activate UI tutorial on first platform of run
        if (gameManager.showTutorial && !tutorialManager.shownUITutorial)
        {
            tutorialManager.ActivateUITutorial();
        }
        // if done, end run (and skip the rest of the procedure)
        if (runState.done)
        {
            gameManager.EndRun(runState);
            return;
        }
        // trigger end activity animation
        player.GetComponent<Animator>().SetTrigger("finishActivity");

        // Zoom out for jump
        camera.ZoomOut();

        if (activityPlatform != null)
        {
            // stop rhythm game
            if (runState.CurrentActivityPlatform())
            {
                rhythmManager.StopRhythm();
            }
        }

        // regenerate energy - ABORTED DUE TO ENERGY = COMBO
        // runState.IncreaseEnergy(gameManager.profile.energyRegen);

        // cap energy
        // runState.energy = System.Math.Min(runState.energy, gameManager.profile.energyCap);

        // emotions take effect on difficulty of jumping to activities
        // (by adjusting height of current platform)
        activityPlatform.Raise(runState.emotions.GetRaiseAmount());

        // spawn new set of platforms
        foreach (Activity activity in SelectActivities())
        {
            SpawnPlatform(activity);
        }
        // clear out all other animation triggers
        player.GetComponent<Animator>().ResetTrigger("startJump");
        player.GetComponent<Animator>().ResetTrigger("activityFail");
        player.GetComponent<Animator>().ResetTrigger("startActivity");
    }

    // called from player controller after sensing ready to jump
    // may be called again from thought menu if wanting to offer thoughts again
    public void PreJump()
    {
        // offer thoughts
        thoughtMenu.Activate(SelectThoughts());
        // activate thought tutorial on first platform of run
        if (gameManager.showTutorial && !tutorialManager.shownThoughtTutorial)
        {
            tutorialManager.ActivateThoughtTutorial();
        }
    }

    // called from thought menu after selecting a thought
    public void PostThoughtSelect()
    {
        /* refill available platforms in case any were deleted
        foreach (Activity activity in SelectActivities())
        {
            SpawnPlatform(activity);
        }*/
        player.Jump();
    }

    // instantiate a new activity platform
    private void SpawnPlatform(Activity activity)
    {
        GameObject platform = Instantiate(platformPrefab);
        platform.GetComponent<ActivityPlatform>().Initialize(activity);
        // add it to list of prospective platforms in runState
        runState.spawnedPlatforms.Add(platform.GetComponent<ActivityPlatform>());
    }

    // select activities from pool of available
    private List<Activity> SelectActivities()
    {      
        List<Activity> offeredActivities = new List<Activity>();
        // get all available normal activities
        List<Activity> availableActivities = new List<Activity>();
        foreach (Activity activity in gameManager.profile.activities)
        {
            for (int i = 0; i < activity.Availability(runState); i++)
            {
                availableActivities.Add(activity);
            }
        }
        // Pick which activities to actually offer, one at a time (random order)
        availableActivities = availableActivities.OrderBy(x => Random.value).ToList();
        // Put scheduled activity at front of list so it is always offered
        Activity scheduledActivity = gameManager.profile.GetSchedule(runState.timeSteps + 1);
        availableActivities.Insert(0, scheduledActivity);
        foreach (Activity available in availableActivities)
        {
            // don't offer activities that are too crammed together
            bool crammed = false;
            foreach (Activity offered in offeredActivities.Concat(runState.spawnedPlatforms.Select(x => x.activity)))
            {
                int h1 = offered.HeightRating(runState);
                int h2 = available.HeightRating(runState);
                if (System.Math.Abs(h1 - h2) < minPlatformHeightDiff)
                {
                    crammed = true;
                }
                // dont add activities that are above the scheduled one
                /* if (h2 - scheduledActivity.HeightRating(runState) > 0)
                {
                    crammed = true;
                } */
            }
            // also dont add breakdown activities
            if (!crammed && !available.isBreakdown)
            {
                offeredActivities.Add(available);
            }
        }
        // There's got to be one default activity
        List<Activity> allActivities = offeredActivities.Concat(runState.spawnedPlatforms.Select(x => x.activity)).ToList();        
        if (allActivities.Where(a => a.IsDefault(runState)).ToList().Count == 0)
        {
            Activity defaultActivity = availableActivities.Find(a => a.IsDefault(runState));
            if (defaultActivity == null)
            {
                defaultActivity = Object.FindObjectOfType<DoNothing>();
            }
            offeredActivities.Add(defaultActivity);
        }
        // lastly, add one breakdown activity
        Debug.Assert(allActivities.Where(a => a.isBreakdown).ToList().Count == 0);
        Activity breakdownActivity = availableActivities.Find(a => a.isBreakdown);
        if (breakdownActivity == null)
        {
            // right now default to "Meditation"
            breakdownActivity = Object.FindObjectOfType<Meditation>();
        }
        offeredActivities.Add(breakdownActivity);

        return offeredActivities;
    }

    // select thoughts from pool of available
    private List<Thought> SelectThoughts()
    {
        // get all available thoughts
        List<Thought> availableThoughts = new List<Thought>();
        foreach (Thought thought in gameManager.profile.thoughts)
        {
            for (int i=0; i < thought.Availability(runState); i++)
            {
                availableThoughts.Add(thought);
            }
        }
        // associated thoughts are extra likely - TODO: tune, maybe use different scheme?
        if (runState.CurrentActivity())
        {
            foreach (Thought thought in runState.CurrentActivity().associatedThoughts)
            {
                for (int i = 0; i < thought.Availability(runState); i++)
                {
                    availableThoughts.Add(thought);
                }
            }
        }
            
        // if none available, return special filler thought
        if (availableThoughts.Count == 0)
        {
            // right now it's called "Nothing"
            Thought fallBack = Object.FindObjectOfType<Nothing>();
            return new List<Thought> { fallBack };
        }
        // select <=3 randomly (without repeat)
        List<Thought> offeredThoughts = new List<Thought>();
        while (offeredThoughts.Count < 3 && availableThoughts.Count > 0)
        {
            int r = Random.Range(0, availableThoughts.Count);
            Thought t = availableThoughts[r];
            availableThoughts.Remove(t);
            offeredThoughts.Add(t);
        }
        return offeredThoughts;
    }
}
