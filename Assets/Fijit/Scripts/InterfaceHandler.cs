using UnityEngine;

public class InterfaceHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovementScript player;
    
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
