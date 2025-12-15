using UnityEngine;

public class EnemySummon : MonoBehaviour
{
    [SerializeField]private GameObject[] enemyPrefab;
    [SerializeField]private Transform[] summonPoints;

    [SerializeField]private float spawnPaddingX = 1f;
    [SerializeField]private float spawnPaddingY = 1f;

    public float summonInterval = 10f;
    private float summonTimer;

    public bool AutomaticSummon = false;
    private void Update()
    {
        summonTimer += Time.deltaTime;

        if (summonTimer >= summonInterval && AutomaticSummon)
        {
            SummonEnemies();
            summonTimer = 0f;
        }
    }
    private void SummonEnemies()
    {
        int i = Random.Range(0, enemyPrefab.Length);
        if (summonPoints!=null)
        {
            
            foreach (Transform point in summonPoints)
            {
                Instantiate(enemyPrefab[i], point.position, point.rotation);
            }
        }
        else
        {
            Vector3 padding = new(spawnPaddingX,spawnPaddingY,0f);

            Instantiate(enemyPrefab[i], transform.position + padding, transform.rotation);
            
        }
        
    }

    public void ManualSummon()
    {
        SummonEnemies();
    }
}
