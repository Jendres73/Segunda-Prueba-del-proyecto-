using UnityEngine;

public class nsssss : MonoBehaviour
{
    public float attackRange = 2f;    // Distancia para atacar
    public float attackCooldown = 1f; // Tiempo entre ataques
    public float attackDamage = 10f;  // Daño por golpe
    public string enemyTag = "Enemy"; // Etiqueta para identificar enemigos
    private float nextAttackTime;
    private Transform targetEnemy;
    void Update()
    {
        // 1. Buscar enemigos cercanos
        FindClosestEnemy();

        // 2. Si tenemos un objetivo y estamos a rango, atacar
        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.position);
            
            if (distance <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
    }
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float closestDist = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestEnemy = enemy.transform;
            }
        }
        targetEnemy = closestEnemy;
    }
    void Attack()
    {
        HealthComponent enemyHealth = targetEnemy.GetComponent<HealthComponent>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
            Debug.Log(gameObject.name + " atacó a " + targetEnemy.name);
        }
    }
    void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}
}