using UnityEngine;
using UnityEngine.SceneManagement;

public class InterfaceHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovementScript player;

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        player.GameOver();
    }
}
