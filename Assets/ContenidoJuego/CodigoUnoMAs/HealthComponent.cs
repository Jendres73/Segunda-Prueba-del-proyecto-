using UnityEngine;
using System.Collections;

public class HealthComponent : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Desactivamos todo ANTES de la corutina
        if (anim != null) anim.SetTrigger("isDead");

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script.GetType().Name == "UnitController" || script.GetType().Name == "EnemyPatrol")
                script.enabled = false;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // INICIAR CORUTINA
        StartCoroutine(DestroyAndNotify());
    }

    IEnumerator DestroyAndNotify()
    {
        // 1. Esperamos la animación
        yield return new WaitForSeconds(1f);

        // 2. ELIMINAMOS EL SCRIPT QUE LO IDENTIFICA COMO ENEMIGO ANTES DE DESTRUIR
        // Esto evita que el Manager lo vea, incluso si el objeto sigue ahí un frame más

        // 3. Destruimos el objeto
        Destroy(gameObject);

        // 4. Esperamos al siguiente frame para asegurar que Unity actualizó la jerarquía
        yield return new WaitForEndOfFrame();

        // 5. Ahora sí, le pedimos al manager que actualice
        
    }
}