using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rhythm Timeline", menuName = "Rhythm/Timeline")]
public class RhythmTimeline : ScriptableObject
{
    public float timelineLength = 180f; // Default to 3 min (180s)
    public List<TimelineEvent> events = new List<TimelineEvent>(); // List of events
}

