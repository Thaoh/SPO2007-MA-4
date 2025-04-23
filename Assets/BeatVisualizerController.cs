using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeatVisualizerController : MonoBehaviour
{
    [Serializable]
    private struct BeatMat
    {
        public Material mat;
        public float defaultSize;
    }
    
    [SerializeField] private BeatMat[] beatMats;
    private float t = 0;
    [SerializeField] private float BeatDecaySpeed = 1;
    
    [SerializeField] private float beatSize = 0.1f;

    private void Start()
    {
        GameManager.OnBeatTriggered.AddListener(NewBeat);
    }

    private void OnDestroy()
    {
        GameManager.OnBeatTriggered.RemoveListener(NewBeat);
    }

    private void Update()
    {
        if (t < 1)
        {
            t += Time.deltaTime * BeatDecaySpeed;
            foreach (var materials in beatMats)
            {
                float wireframeWidth = Mathf.Lerp(beatSize, materials.defaultSize, t);
                materials.mat.SetFloat(Shader.PropertyToID("_WireframeVal"), wireframeWidth);
            }
        }
        else if (t > 1)
        {
            t = 1;
        }
    }

    private void NewBeat()
    {
        StartCoroutine(DelayedBeat());
    }
    
    [SerializeField] private float delayInMilliseconds = 0;

    private IEnumerator DelayedBeat()
    {
        if (delayInMilliseconds < 0) delayInMilliseconds = 0;
        yield return new WaitForSeconds(delayInMilliseconds * 0.0001f);
        t = 0;
    }
}
