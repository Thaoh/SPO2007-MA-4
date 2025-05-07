using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    
    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneName);
    }
}
