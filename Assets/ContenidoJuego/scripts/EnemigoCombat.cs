using UnityEngine;
[RequireComponent(typeof(MovimientoEnemigo))]
[RequireComponent(typeof(Animator))]
public class EnemigoCombat : MonoBehaviour
{
    [Header("Ataque")]
    public Transform puntoAtaque;
    public float radioAtaque = 1f;
    public int daño = 10;
    [Header("Detección")]
    public string tagObjetivo = "Jugador"; // Asegúrate que el Jugador tenga este Tag
    public float rangoDeteccion = 6f;
    public float tiempoEntreAtaques = 1f;
    private Transform objetivo;
    private MovimientoEnemigo movimiento;
    private Animator animator;
    private bool atacando = false;
    private bool esperando = false;
    private float siguienteAtaque;
    void Start()
    {
        movimiento = GetComponent<MovimientoEnemigo>();
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
            BuscarObjetivo();
            return;
        }
        float distancia = Vector2.Distance(transform.position, objetivo.position);
        if (distancia > rangoDeteccion)
        {
            objetivo = null;
            return;
        }
        if (distancia > radioAtaque + 0.5f)
        {
            movimiento.DetenerMovimiento(false);
            movimiento.MoverHacia(objetivo.position);
        }
        else
        {
            movimiento.DetenerMovimiento(true);
            if (!atacando)
            {
                animator.SetBool("Attack", true);
                atacando = true;
            }
        }
    }
    void BuscarObjetivo()
    {
        GameObject[] objetivos = GameObject.FindGameObjectsWithTag(tagObjetivo);
        float distanciaMenor = Mathf.Infinity;

        foreach (GameObject obj in objetivos)
        {
            float distancia = Vector2.Distance(transform.position, obj.transform.position);
            if (distancia < distanciaMenor && distancia <= rangoDeteccion)
            {
                distanciaMenor = distancia;
                objetivo = obj.transform;
            }
        }
    }

    // EVENTO DE ANIMACIÓN: Se llama desde el archivo de animación
    public void Golpear()
    {
        if (puntoAtaque == null)
        {
            Debug.LogWarning(name + ": puntoAtaque no asignado en EnemigoCombat");
            return;
        }

        Collider2D[] golpes = Physics2D.OverlapCircleAll(puntoAtaque.position, radioAtaque);

        if (golpes.Length == 0)
        {
            Debug.Log("El enemigo no detectó ningún collider en el radio de ataque.");
            return;
        }

        bool impacto = false;

        foreach (Collider2D golpe in golpes)
        {
            if (golpe == null || !golpe.CompareTag(tagObjetivo))
                continue;

            Vida vida = golpe.GetComponentInParent<Vida>();

            if (vida != null)
            {
                vida.TomarDaño(daño);
                Debug.Log("El enemigo golpeó a " + golpe.name + " y causó " + daño + " de daño.");
                impacto = true;
            }
            else
            {
                Debug.LogWarning("El objetivo " + golpe.name + " tiene el tag correcto pero no tiene el script Vida.");
            }
        }

        if (!impacto)
        {
            Debug.Log("No se encontró ningún objetivo con tag " + tagObjetivo + " dentro del radio de ataque.");
        }
    }
    public void TerminoAtaque()
    {
        atacando = false;
        animator.SetBool("Attack", false);
        movimiento.DetenerMovimiento(false);
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