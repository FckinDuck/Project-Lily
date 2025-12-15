using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private AudioClip respawnSound;
    private PlayerHealth playerHealth;
    private GameOverManager GameOverManager;
    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        GameOverManager = FindFirstObjectByType<GameOverManager>();
    }

    public void RespawnPlayer()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        

            playerHealth.ResetHealth();

            if (respawnSound != null)
            {
                AudioSource.PlayClipAtPoint(respawnSound, transform.position);
            }
        }else
        {
            GameOverManager.GameOver();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Checkpoint")
        {
            respawnPoint = collision.transform;
            if (respawnSound!=null)
            {
                SoundFXManager.instance.PlaySoundFX(respawnSound, transform,1f);
            }
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("Lit");
        }
    }
}
