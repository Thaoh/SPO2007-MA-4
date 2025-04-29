using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static UnityEvent OnBeatTriggered = new UnityEvent();
    
    [SerializeField] private float noteDuration = 0.3f; // Duration the Input is "active"
    [SerializeField] private GameObject metronomeStickTarget;
    [SerializeField] private GameObject metroStickPrefab;
    [SerializeField] private GameObject canvasObject , camObject, camPivotObject, mapHalo;
    [SerializeField] private Animator postAnim;
    [SerializeField] private float mapMoveSpeed = 2f;
    private ScoreManager scoremanager;

    [SerializeField] private float rotationSpeed = 18f;
    private float targetRotation = 0f, currentRotation = 0f;

    [SerializeField] private Animator playerAnim;


    private AudioSource myAudioSource;
    private bool isClickable = false;
    private int camPosition = 0;

    public List<float> beats = new();

    private void Awake()
    {
        scoremanager = FindFirstObjectByType<ScoreManager>();
    }

    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
    }
    

    void Update()
    {
        for (int i = 0; i < beats.Count; i++)
        {
            if (beats[i] > -noteDuration)
            {
                beats[i] -= Time.deltaTime;
                if (beats[0] < noteDuration)
                {
                    isClickable = true;
                }
                else
                {
                    isClickable = false;
                }
            }
            else
            {
                OnBeatTriggered?.Invoke();
                beats.Remove(beats[i]);
            }
        }

        


        if (Input.GetMouseButtonDown(0))
        {
            clickScreen();
        }

        // Smoothly rotate towards the target rotation
        currentRotation = Mathf.LerpAngle(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        camPivotObject.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

        // spin the halo
        mapHalo.transform.transform.Rotate(Vector3.up * (mapMoveSpeed * Time.deltaTime));
    }

    void clickScreen()
    {
        if (isClickable)
        {
            beats.Remove(beats[0]);
            scoremanager.ScoreChange(5);
            postAnim.SetTrigger("Hit");
            if (Input.mousePosition.x >= 700)
            {
                targetRotation += 45f;
                playerAnim.SetTrigger("TurnRight");
            }
            else
            {
                targetRotation -= 45f;
                playerAnim.SetTrigger("TurnLeft");
            }
            isClickable = false;
        }
        else
        {
            scoremanager.ScoreChange(-2.5f);
            Handheld.Vibrate();
            Debug.Log("Miss! Too early or late.");
        }
    }

    public void EventTrigger()
    {
        SpawnMetroLine();
        beats.Add(1);
        
    }

    public void PlayAudioOneShot()
    {
        //myAudioSource.PlayOneShot(myAudioSource.clip);
    }

    void SpawnMetroLine()
    {
        // first line
        GameObject Go = Instantiate(metroStickPrefab, metronomeStickTarget.transform.position, Quaternion.identity,canvasObject.transform);
        Go.transform.localPosition = new Vector3(500, metronomeStickTarget.transform.localPosition.y, 0);
        StickBehaviour stick = Go.GetComponent<StickBehaviour>();
        stick.targetLocation = metronomeStickTarget.transform;
        stick.gm = this;
        stick.shouldGiveTrigger = true;
        stick.timeToMove = 1f;
        stick.moveLeft = true;

        // second line
        GameObject Go2 = Instantiate(metroStickPrefab, metronomeStickTarget.transform.position, Quaternion.identity,canvasObject.transform);
        Go2.transform.localPosition = new Vector3(-500, metronomeStickTarget.transform.localPosition.y, 0);
        StickBehaviour stick2 = Go2.GetComponent<StickBehaviour>();
        stick2.targetLocation = metronomeStickTarget.transform;
        stick2.gm = this;
        stick2.shouldGiveTrigger = false;
        stick2.timeToMove = 1f;
        stick2.moveLeft = false;

    }

}
