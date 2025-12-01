using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Main Menu Object")]
    [SerializeField] private GameObject _loadingBarObject;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private GameObject[] _objectToHide;

    [Header("Scene To Load")]
    [SerializeField] private SceneField _presitenceGameplay;
    [SerializeField] private SceneField _levelScene;

    private List<AsyncOperation> _sceneToLoad = new List<AsyncOperation>();

    private void Awake()
    {
        _loadingBarObject.SetActive(false);
    }

    public void StartGame()
    {
        HideMenu();

        _loadingBarObject.SetActive(true);

        _sceneToLoad.Add(SceneManager.LoadSceneAsync(_presitenceGameplay));
        _sceneToLoad.Add(SceneManager.LoadSceneAsync(_levelScene,LoadSceneMode.Additive));

        StartCoroutine(ProgressLoadingBar());
    }
    private void HideMenu()
    {
        for (int i = 0; i < _objectToHide.Length; i++)
        {
            _objectToHide[i].SetActive(false);
        }
    }
    private IEnumerator ProgressLoadingBar()
    {
        float progress = 0f;
        for (int i = 0; i < _sceneToLoad.Count; i++)
        {
            while (_sceneToLoad[i].isDone)
            {
                progress+= _sceneToLoad[i].progress;
                _loadingBar.fillAmount = progress / _sceneToLoad.Count;
                yield return null;
            }

        }
    }
}
