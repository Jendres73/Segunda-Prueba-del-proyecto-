using UnityEngine;
public class MovimientoEnemigo : MonoBehaviour
{
    [Header("Patrulla")]
    public Transform[] puntosPatrulla;
    public float velocidad = 3f;
    public float distanciaCambio = 0.2f;
    private int puntoActual = 0;
    private bool detenido = false;
    private bool persiguiendo = false;
    private Vector2 destino;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (detenido)
        {
            animator.SetBool("Caminando", false);
            return;
        }
        if (persiguiendo)
        {
            MoverAlObjetivo();
        }
        else
        {
            Patrullar();
        }
    }
    private void Patrullar()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0)
            return;
        Transform punto =
            puntosPatrulla[puntoActual];
        transform.position = Vector2.MoveTowards(
            transform.position,
            punto.position,
            velocidad * Time.deltaTime
        );
        GirarSprite(punto.position.x);
        animator.SetBool(
            "Caminando",
            true
        );
        if(Vector2.Distance(
            transform.position,
            punto.position) <= distanciaCambio)
        {
            puntoActual++;

            if(puntoActual >= puntosPatrulla.Length)
            {
                puntoActual = 0;
            }
        }
    }
    public void MoverHacia(Vector2 posicion)
    {
        destino = posicion;

        persiguiendo = true;

        detenido = false;
    }
    private void MoverAlObjetivo()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.deltaTime
        );
        GirarSprite(destino.x);
        animator.SetBool(
            "Caminando",
            true
        );
    }
    public void DetenerMovimiento(bool estado)
    {
        detenido = estado;

        animator.SetBool(
            "Caminando",
            !estado
        );
    }
    public void Detener()
    {
        animator.SetBool(
            "Caminando",
            false
        );
    }
    public void CancelarPersecucion()
    {
        persiguiendo = false;

        detenido = false;
    }
    private void GirarSprite(float objetivoX)
    {
        if(spriteRenderer == null)
            return;


        if(objetivoX < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if(puntosPatrulla == null)
            return;
        Gizmos.color = Color.red;
        foreach(Transform punto in puntosPatrulla)
        {
            if(punto != null)
            {
                Gizmos.DrawSphere(
                    punto.position,
                    0.15f
                );
            }
        }
    }
}