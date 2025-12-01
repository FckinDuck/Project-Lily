using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager Instance;
    private static bool _loadFromDoor;
    private DoorTriggerInteraction.DoorToSpawnAt _doorToSpawnTo;
    private GameObject _player;
    private Collider2D _playercoll;
    private Collider2D _doorcoll;
    private Vector3 _playerSpawnLoc;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _player = GameObject.FindGameObjectWithTag("Player");
        _playercoll = _player.GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void SwapSceneFromDoorUse(SceneField sceneField, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt)
    {
        _loadFromDoor = true;
        Instance.StartCoroutine(Instance.FadeOutThenChangeScene(sceneField, doorToSpawnAt));
    }
    private IEnumerator FadeOutThenChangeScene(SceneField sceneField, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None)
    {
        //fading

        SceneFadeManager.instance.StartFadeOut();

        while(SceneFadeManager.instance.IsFadeOut)
        {
            yield return null;
        }

        _doorToSpawnTo = doorToSpawnAt;
        SceneManager.LoadScene(sceneField);

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneFadeManager.instance.StartFadeIn();
        if (_loadFromDoor)
        {
            FindDoor(_doorToSpawnTo);
            _player.transform.position = _playerSpawnLoc;
            _loadFromDoor= false;
        }
    }

    private void FindDoor(DoorTriggerInteraction.DoorToSpawnAt doorSpawnNumber)
    {
        DoorTriggerInteraction[] doors = FindObjectsOfType<DoorTriggerInteraction>();
        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].CurrentDoorPosition == doorSpawnNumber)
            {
                _doorcoll = doors[i].gameObject.GetComponent<Collider2D>();

                CalculateSpawnPos();
                return;
            }
        }
    }

    private void CalculateSpawnPos()
    {
        float colliderHeight = _playercoll.bounds.extents.y;
        _playerSpawnLoc = _doorcoll.transform.position + new Vector3(0f,colliderHeight,0f );
    }


}

