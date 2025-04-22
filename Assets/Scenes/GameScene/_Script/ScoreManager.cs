using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float score = 0f;

    private void Start()
    {
        scoreText.text = "Score - " + score;
    }

    public void ScoreChange(float value)
    {
        score += value;
        scoreText.text = "Score " + score;
    }
}