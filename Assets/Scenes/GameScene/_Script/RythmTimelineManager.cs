using UnityEngine;
using System.Collections.Generic;

public class RhythmTimelineManager : MonoBehaviour
{
    public RhythmTimeline timeline;
    public float bpm = 120f;

    private float beatInterval;
    private float startTime , waitABit = 5f;
    private bool songPlaying = false;
    private List<TimelineEvent> pendingEvents;

    [SerializeField] private GameManager gameManager;

    void Start()
    {
        if (timeline == null)
        {
            Debug.LogError("No RhythmTimeline assigned!");
            return;
        }

        beatInterval = 60f / bpm;
        pendingEvents = new List<TimelineEvent>(timeline.events);
    }

    void Update()
    {
        if(waitABit > 0)
        {
            waitABit -= Time.deltaTime;

            if(waitABit <= 1f && !songPlaying)
            {
                GetComponent<AudioSource>().Play();
                startTime = Time.time -0.5f;
                songPlaying = true;
            }
            return;
        }

        
        float elapsedTime = Time.time - startTime;
        float currentBeat = elapsedTime / beatInterval;

        for (int i = pendingEvents.Count - 1; i >= 0; i--)
        {
            if (currentBeat >= pendingEvents[i].beatPosition)
            {
                PlayEvent(pendingEvents[i]);
                pendingEvents.RemoveAt(i);
            }
        }
    }

    void PlayEvent(TimelineEvent evt)
    {
        gameManager.EventTrigger();
    }
}
