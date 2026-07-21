using UnityEngine;

public class Collector : MonoBehaviour
{
    [Header("Configuración de Recolección")]
    public int capacity = 10;
    public int currentLoad = 0;
    public int currentState = 0; // 0: Idle, 1: Ir a Recurso, 2: Recolectando, 3: Ir a Entrega

    private Transform targetResource;
    private Transform dropoffBuilding;

    void Update()
    {
        // NOTA: Si el jugador dio una orden de movimiento manual (clic derecho en suelo),
        // el isPlayerCommanding será true, por lo que este script se pausará.
        // Si queremos que la recolección sea automática, ignoramos el isPlayerCommanding 
        // solo si estamos en medio de un proceso de recolección.
        
        switch (currentState)
        {
            case 1: // Yendo al recurso
                if (targetResource == null) { currentState = 0; return; }
                
                GetComponent<UnitMovement>().SetNewDestination(targetResource.position);
                
                // Si estamos cerca, empezamos a recolectar
                if (Vector2.Distance(transform.position, targetResource.position) < 1.5f)
                    currentState = 2;
                break;

            case 2: // Recolectando (Bucle de carga)
                if (targetResource != null)
                {
                    Resource res = targetResource.GetComponent<Resource>();
                    if (res != null) {
                        currentLoad += res.Harvest(capacity - currentLoad);
                        
                        // Si ya está llena, vamos al edificio
                        if (currentLoad >= capacity) currentState = 3;
                    }
                }
                else currentState = 0; // Si el recurso desapareció, paramos
                break;

            case 3: // Yendo a entregar
                if (dropoffBuilding == null) 
                    dropoffBuilding = GameObject.FindGameObjectWithTag("Building").transform;

                GetComponent<UnitMovement>().SetNewDestination(dropoffBuilding.position);

                // Si estamos cerca, entregamos
                if (Vector2.Distance(transform.position, dropoffBuilding.position) < 2.0f)
                {
                    currentLoad = 0;
                    currentState = 1; // Volvemos a buscar el recurso (Bucle infinito de trabajo)
                }
                break;
        }
    }

    public void StartCollecting(Transform resource)
    {
        targetResource = resource;
        // Buscamos el edificio la primera vez que inicia la orden
        GameObject b = GameObject.FindGameObjectWithTag("Building");
        if (b != null) dropoffBuilding = b.transform;
        
        currentState = 1;
        // Liberamos el mando para que el Update pueda mover la unidad
        GetComponent<UnitController>().isPlayerCommanding = false;
    }
}