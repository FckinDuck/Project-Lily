using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeManager : MonoBehaviour
{
    public static SceneFadeManager instance;

    [SerializeField] private Image _fadeOutImage;
    [Range(0.1f,10f),SerializeField] private float _fadeOutSpeed =5f;
    [Range(0.1f,10f),SerializeField] private float _fadeInSpeed= 5f;

    [SerializeField] private Color _fadeOutStartColor;

    public bool IsFadeOut { get; private set; }
    public bool IsFadeIn { get; private set; }

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        _fadeOutStartColor.a = 0f;
    }

    private void Update()
    {
        if (IsFadeOut)
        {
            if (_fadeOutImage.color.a < 1f)
            {
                _fadeOutStartColor.a += Time.deltaTime * _fadeOutSpeed;
                _fadeOutImage.color = _fadeOutStartColor;
            }
            else
            {
                IsFadeOut = false;
            }
        }

        if (IsFadeIn)
        {
            if (_fadeOutImage.color.a > 0f)
            {
                _fadeOutStartColor.a -= Time.deltaTime * _fadeInSpeed;
                _fadeOutImage.color = _fadeOutStartColor;
            }
            else
            {
                IsFadeIn = false;
            }
        }
    }

    public void StartFadeOut()
    {
        _fadeOutImage.color = _fadeOutStartColor;
        IsFadeOut = true;
    }
    public void StartFadeIn()
    {
        if(_fadeOutImage.color.a >=1f)
        {
            _fadeOutImage.color = _fadeOutStartColor;
            IsFadeIn = true;

        }
    }

}
