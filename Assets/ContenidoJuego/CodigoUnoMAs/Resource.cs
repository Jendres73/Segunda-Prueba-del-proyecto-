using UnityEngine;

public class Resource : MonoBehaviour
{
    public string resourceType = "Oro";
    public int amount = 100;

    public int Harvest(int amountToTake)
    {
        int taken = Mathf.Min(amountToTake, amount);
        amount -= taken;

        // Buscamos al gestor en la escena
        ResourceManager manager = Object.FindFirstObjectByType<ResourceManager>();
        
        if (manager != null)
        {
            manager.AddCollectedResources(taken);
        }
        else
        {
            Debug.LogError("No se encuentra el objeto ResourceManager en la escena. ¿Aseguraste de tener un objeto con ese script?");
        }

        if (amount <= 0) 
        {
            Destroy(gameObject);
        }
        
        return taken;
    }
}