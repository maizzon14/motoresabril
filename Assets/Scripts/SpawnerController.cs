using System.Collections;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Enemy;

    [SerializeField] float SpawnRange = 2f;
    [SerializeField] float SpawnCooldown = 5f;

    bool stopSpawner = false;

    void Start()
    {
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(SpawnCooldown);

        while(!stopSpawner)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(SpawnCooldown);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SpawnRange);
    }

    void SpawnEnemy()
    {
        float distance = Random.Range(0, SpawnRange);
        float angle = Random.Range(0, 360);

        Vector3 newPosition = transform.position 
            + Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        GameObject spawn = Instantiate(Enemy, newPosition, Quaternion.identity);

        spawn.GetComponent<EnemyController>().SetTarget(Player);
    }
}
