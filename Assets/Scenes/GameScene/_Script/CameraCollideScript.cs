using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraCollideScript : MonoBehaviour
{
    private ScoreManager scoremanager;

    private void Awake()
    {
        scoremanager = FindFirstObjectByType<ScoreManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            scoremanager.ScoreChange(-5);
            Handheld.Vibrate();
        }
    }
}
