using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private bool isPaused = false;
    [SerializeField] private GameObject pausedPanel;
    void Update()
    {
        if (InputManager.Pause)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }
    }
    public void Paused()
    {
        isPaused = true;
        pausedPanel.SetActive(true);
        Time.timeScale = 0f;

        InputManager.PlayerInput.enabled = false;
    }

    public void Resume()
    {
        isPaused = false;
        pausedPanel.SetActive(false);
        Time.timeScale = 1f;

        InputManager.PlayerInput.enabled = true;    
    }
}
