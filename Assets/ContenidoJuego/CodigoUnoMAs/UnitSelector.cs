using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    public bool isSelected = false;
    public GameObject selectionMarker;
    
    // Añadimos una referencia al componente de movimiento
    private UnitMovement movement;

    void Start()
    {
        movement = GetComponent<UnitMovement>(); // Obtenemos la referencia
        if (selectionMarker != null) selectionMarker.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (selectionMarker != null) selectionMarker.SetActive(selected);
        
        // CORRECCIÓN: Sincronizamos con UnitMovement
        if (movement != null)
        {
            movement.isSelected = selected;
        }
    }
}