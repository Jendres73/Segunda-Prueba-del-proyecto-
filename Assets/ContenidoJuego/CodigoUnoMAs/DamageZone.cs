using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private float tiempoEntreDaño = 1f; // Segundos entre cada golpe
    [SerializeField] private float dañoPorTick = 10f;
    
    private float tiempoSiguienteDaño;
    private bool unidadEnZona = false;
    private HealthComponent unidadVida;

    private void Update()
    {
        // El temporizador corre en el Update, garantizando daño constante
        if (unidadEnZona && unidadVida != null)
        {
            tiempoSiguienteDaño -= Time.deltaTime;

            if (tiempoSiguienteDaño <= 0)
            {
                unidadVida.TakeDamage(dañoPorTick);
                tiempoSiguienteDaño = tiempoEntreDaño; // Reinicia el ciclo
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Buscamos el componente de vida en lo que entre a la zona
        HealthComponent vida = other.GetComponent<HealthComponent>();
        
        if (vida != null)
        {
            unidadVida = vida;
            unidadEnZona = true;
            
            // Daño instantáneo al entrar
            unidadVida.TakeDamage(dañoPorTick);
            tiempoSiguienteDaño = tiempoEntreDaño;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<HealthComponent>() != null)
        {
            unidadEnZona = false;
            unidadVida = null;
        }
    }
}