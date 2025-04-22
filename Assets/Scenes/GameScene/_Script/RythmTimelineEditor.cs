# if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RhythmTimeline))]
public class RhythmTimelineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RhythmTimeline timeline = (RhythmTimeline)target;

        // Timeline Length
        timeline.timelineLength = EditorGUILayout.FloatField("Timeline Length (s)", timeline.timelineLength);

        // Display & Edit Events
        EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);

        for (int i = 0; i < timeline.events.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            timeline.events[i].beatPosition = EditorGUILayout.FloatField("Beat", timeline.events[i].beatPosition);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                timeline.events.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        // Add New Event Button
        if (GUILayout.Button("Add Event"))
        {
            timeline.events.Add(new TimelineEvent { beatPosition = 0 });
        }

        // Save Changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(timeline);
        }
    }
}

# endif

