using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = 0.5f;
    [SerializeField] private Gradient _healthBarGradient;

    private Color targetColorAfterHit;
    private float targetHealthAfterHit =1f;

    private Coroutine drainHealthBarCoroutine;

    private Image _healthBarImage;

    void Start()
    {
        _healthBarImage = GetComponent<Image>();

        _healthBarImage.color = _healthBarGradient.Evaluate(targetHealthAfterHit);

        CheckHealthBarGradienAmount();
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        targetHealthAfterHit = currentHealth / maxHealth;
        drainHealthBarCoroutine = StartCoroutine(DrainHealthBar());
        CheckHealthBarGradienAmount();
    }

    private IEnumerator DrainHealthBar()
    {
        float elapsed = 0f;
        float initialFill = _healthBarImage.fillAmount;
        Color currentColor = _healthBarImage.color;

        while (elapsed < _timeToDrain)
        {
            elapsed += Time.deltaTime;
            _healthBarImage.fillAmount = Mathf.Lerp(initialFill, targetHealthAfterHit, elapsed / _timeToDrain);
            _healthBarImage.color = Color.Lerp(currentColor, targetColorAfterHit, elapsed / _timeToDrain);
            yield return null;
        }

    }

    private void CheckHealthBarGradienAmount()
    {
        targetColorAfterHit = _healthBarGradient.Evaluate(targetHealthAfterHit);
    }
}
