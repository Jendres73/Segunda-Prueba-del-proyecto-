using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class Movimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    [Header("Selección")]
    public Sprite imagenSeleccion;
    public Color colorSeleccion = new Color(0f,1f,0f,0.5f);
    public Vector2 desplazamientoCirculo = new Vector2(0,-0.5f);
    public float escalaDeLaImagen = 1f;
    private bool seleccionada = false;
    private Vector2 destino;
    private bool moviendose = false;
    private bool bloqueado = false;
    private GameObject objetoSeleccion;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        destino = transform.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CrearSeleccion();
    }
    void Update()
    {
        if(Mouse.current == null)
            return;
        Seleccion();
        OrdenMovimiento();
        Mover();
    }
    private void Seleccion()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 posicionMouse =
                Camera.main.ScreenToWorldPoint(
                    Mouse.current.position.ReadValue()
                );
            RaycastHit2D hit =
                Physics2D.Raycast(
                    posicionMouse,
                    Vector2.zero
                );
            if(hit.collider != null &&
               hit.collider.gameObject == gameObject)
            {
                seleccionada = true;
                objetoSeleccion.SetActive(true);
            }
            else
            {
                seleccionada = false;
                objetoSeleccion.SetActive(false);
            }
        }
    }
    private void OrdenMovimiento()
    {
        if(bloqueado)
            return;
        if(seleccionada &&
           Mouse.current.rightButton.wasPressedThisFrame)
        {
            destino =
                Camera.main.ScreenToWorldPoint(
                    Mouse.current.position.ReadValue()
                );
            moviendose = true;
        }
    }
    private void Mover()
    {
        if(bloqueado)
        {
            animator.SetBool(
                "Caminando",
                false
            );
            return;
        }
        if(moviendose)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    destino,
                    velocidad * Time.deltaTime
                );
            if(destino.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
            if(Vector2.Distance(
                transform.position,
                destino) < 0.05f)
            {
                moviendose = false;
            }
        }
        animator.SetBool(
            "Caminando",
            moviendose
        );
    }
    public void BloquearMovimiento(bool estado)
    {
        bloqueado = estado;
        if(estado)
        {
            moviendose = false;
            animator.SetBool(
                "Caminando",
                false
            );
        }
    }
    private void CrearSeleccion()
    {
        objetoSeleccion =
            new GameObject(
                "Seleccion"
            );
        objetoSeleccion.transform.SetParent(
            transform
        );
        objetoSeleccion.transform.localPosition =
            desplazamientoCirculo;
        objetoSeleccion.transform.localScale =
            new Vector3(
                escalaDeLaImagen,
                escalaDeLaImagen,
                1
            );
        SpriteRenderer sr =
            objetoSeleccion.AddComponent<SpriteRenderer>();
        sr.sprite =
            imagenSeleccion;
        sr.color =
            colorSeleccion;
        sr.sortingOrder = -1;
        objetoSeleccion.SetActive(false);
    }
}