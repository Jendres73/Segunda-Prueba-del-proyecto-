using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// V2_ResourceManager.cs - Gestor centralizado de recursos
/// Mantiene registro de todos los recursos en el mapa
/// </summary>
public class V2_ResourceManager : MonoBehaviour
{
    // ==================== SINGLETON ====================
    public static V2_ResourceManager InstanceV2 { get; private set; }

    // ==================== VARIABLES ====================
    private List<V2_Resource> allResourcesV2 = new List<V2_Resource>();
    private Dictionary<ResourceType, int> playerResourcesStoredV2 = new Dictionary<ResourceType, int>();

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogsV2 = false;

    // ==================== EVENTOS ====================
    public event System.Action<ResourceType, int> OnPlayerResourcesChangedV2;

    // ==================== PROPIEDADES ====================
    public int TotalResourcesInMapV2 => allResourcesV2.Count;

    // ==================== UNITY LIFECYCLE ====================
    private void AwakeInitializeV2()
    {
        if (InstanceV2 != null && InstanceV2 != this)
        {
            LogDebugV2("ResourceManager instance already exists. Destroying duplicate.", LogType.Warning);
            Destroy(gameObject);
            return;
        }
        InstanceV2 = this;

        // Inicializar diccionario de recursos del jugador
        foreach (ResourceType typeV2 in System.Enum.GetValues(typeof(ResourceType)))
        {
            playerResourcesStoredV2[typeV2] = 0;
        }

        LogDebugV2("ResourceManager initialized as Singleton");
    }

    private void StartFindResourcesV2()
    {
        V2_Resource[] resourcesInSceneV2 = FindObjectsByType<V2_Resource>(FindObjectsSortMode.None);
        
        foreach (V2_Resource resourceV2 in resourcesInSceneV2)
        {
            RegisterResourceV2(resourceV2);
        }

        LogDebugV2($"Found {allResourcesV2.Count} resources in scene");
    }

    private void Awake()
    {
        AwakeInitializeV2();
    }

    private void Start()
    {
        StartFindResourcesV2();
    }

    // ==================== PUBLIC METHODS ====================

    /// <summary>
    /// Registra un recurso en el gestor
    /// </summary>
    public void RegisterResourceV2(V2_Resource resourceV2)
    {
        if (resourceV2 == null || allResourcesV2.Contains(resourceV2))
            return;

        allResourcesV2.Add(resourceV2);
        LogDebugV2($"Resource registered: {resourceV2.gameObject.name} - Type: {resourceV2.ResourceTypeV2}");
    }

    /// <summary>
    /// Desregistra un recurso
    /// </summary>
    public void UnregisterResourceV2(V2_Resource resourceV2)
    {
        if (resourceV2 != null)
        {
            allResourcesV2.Remove(resourceV2);
            LogDebugV2($"Resource unregistered: {resourceV2.gameObject.name}");
        }
    }

    /// <summary>
    /// Obtiene todos los recursos del tipo especificado
    /// </summary>
    public List<V2_Resource> GetResourcesByTypeV2(ResourceType typeV2)
    {
        return allResourcesV2.Where(r => r.ResourceTypeV2 == typeV2).ToList();
    }

    /// <summary>
    /// Obtiene el recurso más cercano de un tipo específico
    /// </summary>
    public V2_Resource GetNearestResourceV2(Vector3 positionV2, ResourceType typeV2)
    {
        List<V2_Resource> resourcesOfTypeV2 = GetResourcesByTypeV2(typeV2);

        if (resourcesOfTypeV2.Count == 0)
            return null;

        return resourcesOfTypeV2
            .OrderBy(r => Vector3.Distance(r.transform.position, positionV2))
            .FirstOrDefault();
    }

    /// <summary>
    /// Añade recursos al almacén del jugador
    /// </summary>
    public void AddPlayerResourceV2(ResourceType typeV2, int amountV2)
    {
        playerResourcesStoredV2[typeV2] += amountV2;
        OnPlayerResourcesChangedV2?.Invoke(typeV2, playerResourcesStoredV2[typeV2]);
        LogDebugV2($"Added {amountV2} {typeV2}. Total: {playerResourcesStoredV2[typeV2]}");
    }

    /// <summary>
    /// Resta recursos del almacén del jugador
    /// </summary>
    public bool SpendPlayerResourceV2(ResourceType typeV2, int amountV2)
    {
        if (playerResourcesStoredV2[typeV2] >= amountV2)
        {
            playerResourcesStoredV2[typeV2] -= amountV2;
            OnPlayerResourcesChangedV2?.Invoke(typeV2, playerResourcesStoredV2[typeV2]);
            LogDebugV2($"Spent {amountV2} {typeV2}. Total: {playerResourcesStoredV2[typeV2]}");
            return true;
        }

        LogDebugV2($"Not enough {typeV2}. Need: {amountV2}, Have: {playerResourcesStoredV2[typeV2]}", LogType.Warning);
        return false;
    }

    /// <summary>
    /// Obtiene la cantidad de recurso que tiene el jugador
    /// </summary>
    public int GetPlayerResourceAmountV2(ResourceType typeV2)
    {
        return playerResourcesStoredV2[typeV2];
    }

    /// <summary>
    /// Obtiene todos los recursos del jugador
    /// </summary>
    public Dictionary<ResourceType, int> GetAllPlayerResourcesV2()
    {
        return new Dictionary<ResourceType, int>(playerResourcesStoredV2);
    }

    /// <summary>
    /// Obtiene todos los recursos del mapa
    /// </summary>
    public List<V2_Resource> GetAllResourcesV2()
    {
        return new List<V2_Resource>(allResourcesV2);
    }

    // ==================== PRIVATE METHODS ====================

    /// <summary>
    /// Log de debug
    /// </summary>
    private void LogDebugV2(string messageV2, LogType typeV2 = LogType.Log)
    {
        if (!enableDebugLogsV2)
            return;

        switch (typeV2)
        {
            case LogType.Warning:
                Debug.LogWarning($"[V2_ResourceManager] {messageV2}");
                break;
            case LogType.Error:
                Debug.LogError($"[V2_ResourceManager] {messageV2}");
                break;
            default:
                Debug.Log($"[V2_ResourceManager] {messageV2}");
                break;
        }
    }
}