using UnityEngine;

[RequireComponent(typeof(Movimiento))]
[RequireComponent(typeof(Animator))]
public class Combate : MonoBehaviour
{
    [Header("Detección")]
    public string tagEnemigo = "Enemigo";
    public float rangoDeteccion = 6f;

    [Header("Ataque")]
    public Transform puntoAtaque;
    public float radioAtaque = 1f;
    public int daño = 10;
    public float tiempoEntreAtaques = 1f;

    private Transform objetivo;
    private Movimiento movimiento;
    private Animator animator;
    private bool atacando = false;
    private bool esperando = false;
    private float siguienteAtaque;

    void Start()
    {
        movimiento = GetComponent<Movimiento>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (esperando)
        {
            if (Time.time >= siguienteAtaque)
                esperando = false;
            else
                return;
        }

        if (objetivo == null)
        {
            BuscarEnemigo();
            return;
        }

        float distancia = Vector2.Distance(transform.position, objetivo.position);

        if (distancia > rangoDeteccion)
        {
            objetivo = null;
            movimiento.BloquearMovimiento(false);
            return;
        }

        if (distancia > radioAtaque + 0.5f)
        {
            movimiento.BloquearMovimiento(false);
        }
        else
        {
            movimiento.BloquearMovimiento(true);
            if (!atacando)
            {
                animator.SetBool("Attack", true);
                atacando = true;
            }
        }
    }

    void BuscarEnemigo()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag(tagEnemigo);
        float distanciaMenor = Mathf.Infinity;

        foreach (GameObject enemigo in enemigos)
        {
            float distancia = Vector2.Distance(transform.position, enemigo.transform.position);
            if (distancia < distanciaMenor && distancia <= rangoDeteccion)
            {
                distanciaMenor = distancia;
                objetivo = enemigo.transform;
            }
        }
    }

    // EVENTO DE ANIMACIÓN: Se llama desde el archivo de animación
    public void Golpear()
    {
        if (puntoAtaque == null)
        {
            Debug.LogWarning(name + ": puntoAtaque no asignado en Combate");
            return;
        }

        Collider2D[] golpes = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque);

        if (golpes.Length == 0)
        {
            Debug.Log("No se detectó ningún collider en el radio de ataque.");
            return;
        }

        bool impacto = false;

        foreach (Collider2D golpe in golpes)
        {
            if (golpe == null || !golpe.CompareTag(tagEnemigo))
                continue;

            Vida vida = golpe.GetComponentInParent<Vida>();

            if (vida != null)
            {
                vida.TomarDaño(daño);
                Debug.Log("Golpeaste a " + golpe.name + " y causaste " + daño + " de daño.");
                impacto = true;
            }
            else
            {
                Debug.LogWarning("El objeto " + golpe.name + " tiene el Tag correcto pero no encontró el script Vida.");
            }
        }

        if (!impacto)
        {
            Debug.Log("No se encontró ningún objetivo con tag " + tagEnemigo + " dentro del radio de ataque.");
        }
    }

    public void TerminoAtaque()
    {
        atacando = false;
        animator.SetBool("Attack", false);
        movimiento.BloquearMovimiento(false);
        esperando = true;
        siguienteAtaque = Time.time + tiempoEntreAtaques;
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaque.position, radioAtaque);
        }
    }
}