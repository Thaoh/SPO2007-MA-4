using System;
using UnityEngine;
using UnityEngine.Audio;

public class BeatManager : MonoBehaviour
{
    public static BeatManager instance;

    [SerializeField] private GameManager manager;
    
    [SerializeField] private int bpm;
    private float _beatInterval, _beatTimer;
    public static bool BeatFull;
    private bool _musicStarted = false;
    public static int BeatCountFull;
    
    [SerializeField] private AudioSource musicSource;

    public Action OnBeatHit;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OnBeatHit += manager.EventTrigger;
    }

    public void SetBPM(int bpm)
    {
        this.bpm = bpm;
    }


    private void Update()
    {
        BeatDetection();
    }

    private void BeatDetection()
    {
        BeatFull = false;
        _beatInterval = 60f / bpm;
        _beatTimer += Time.deltaTime;
        if (_beatTimer >= _beatInterval)
        {
            if (!_musicStarted)
            {
                musicSource.Play();
                _musicStarted = true;
            }
            _beatTimer -= _beatInterval;
            OnBeatHit?.Invoke();
            BeatFull = true;
            BeatCountFull++;
        }
    }
}
