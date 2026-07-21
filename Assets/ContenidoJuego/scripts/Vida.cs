using UnityEngine;

public class Vida : MonoBehaviour
{
    [SerializeField] private int vida = 100;
    [SerializeField] private int maximoVida = 100;
    private void Start()
    {
        vida = maximoVida;
    }
    public void TomarDaño(int daño)
    {
        vida -= daño;
        //Debug.Log(gameObject.name + " recibió " + daño + " de daño. Vida restante: " + vida);
        if (vida <= 0)
        {
            vida = 0;
            Morir();
        }
    }
    private void Morir()
    {
        //Debug.Log(gameObject.name + " ha muerto.");
        Destroy(gameObject, 0.1f);
    }
}