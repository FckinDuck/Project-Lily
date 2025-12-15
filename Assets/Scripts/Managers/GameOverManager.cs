using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private AudioClip gameOverSound;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        gameObject.SetActive(false); 
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }
    private void FixedUpdate()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            GameOver();
        }
        else
        {
            Debug.Log("PlayerHealth component is null in GameOverManager");
        }
    }
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        if (gameOverSound != null)
        {
            SoundFXManager.instance.PlaySoundFX(gameOverSound,Camera.main.transform, 0.2f);
        }
    }
    /*
    public void Retry()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    */
    public void Menu()
    {
        SceneManager.LoadScene("StartGame");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
