using UnityEngine;


public class SoundFXManager : MonoBehaviour
{

    [SerializeField] private AudioSource soundFXObject;
    public static SoundFXManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this ;
        }
    }

    public void PlaySoundFX(AudioClip audioClip, Transform transform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);

        audioSource.clip = audioClip;


        audioSource.volume = volume;


        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject,clipLength);
    }

    public void PlayRandomSoundFX(AudioClip[] audioClip, Transform transform, float volume)
    {
        int rand = Random.Range(0,audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObject, transform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];


        audioSource.volume = volume;


        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject,clipLength);
    }
}
