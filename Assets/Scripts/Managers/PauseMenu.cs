using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private bool isPaused = false;
    [SerializeField] private GameObject pausedPanel;
    void Update()
    {
        if (UserInputs.instance.control.UI.Cancel.WasPerformedThisFrame())
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
        
        UserInputs.instance.control.Player.Disable();
    }

    public void Resume()
    {
        isPaused = false;
        pausedPanel.SetActive(false);
        Time.timeScale = 1f;

        UserInputs.instance.control.Player.Enable();
    }
}
