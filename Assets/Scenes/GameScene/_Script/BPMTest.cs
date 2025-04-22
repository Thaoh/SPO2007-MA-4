using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BPMTest : MonoBehaviour
{
    public float bpm = 120f; // Beats per minute
    public float noteDuration = 0.2f; // Duration the note is "active"
    public Button rhythmButton; // UI Button (assigned in inspector)
    public AudioSource noteAudio; // Assign a short note sound

    [SerializeField] private TMP_Text buttonText;

    private float beatInterval; // Time between beats (0.5s for 120 BPM)
    private float nextBeatTime;
    private bool isClickable = false;

    void Start()
    {
        beatInterval = 60f / bpm; // 120 BPM = 2 beats per sec -> 0.5s per beat
        nextBeatTime = Time.time + beatInterval; // Start at first beat

        if (rhythmButton != null)
        {
            rhythmButton.onClick.AddListener(OnButtonClick);
        }

        DisableButton();
    }

    void Update()
    {
        if (Time.time >= nextBeatTime)
        {
            PlayNote();
            nextBeatTime += beatInterval; // Schedule next note
        }
    }

    void PlayNote()
    {
        if (noteAudio != null) noteAudio.Play(); // Play note sound
        EnableButton();
        Invoke(nameof(DisableButton), noteDuration); // Disable button after note duration
    }

    void OnButtonClick()
    {
        if (isClickable)
        {
            Debug.Log("Perfect Hit!");
        }
        else
        {
            Debug.Log("Miss! Too early or late.");
        }
    }

    void EnableButton()
    {
        isClickable = true;
        buttonText.text = "click";
        if (rhythmButton != null) rhythmButton.interactable = true;
    }

    void DisableButton()
    {
        isClickable = false;
        buttonText.text = "wait";
        if (rhythmButton != null) rhythmButton.interactable = false;
    }
}
