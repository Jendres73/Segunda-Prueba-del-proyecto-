using UnityEngine;

/// <summary>
/// V2_Collector.cs - Componente para recolectar recursos
/// Se añade a las unidades del jugador que pueden recolectar
/// </summary>
public class V2_Collector : MonoBehaviour
{
    // ==================== VARIABLES ====================
    [Header("Collection Settings")]
    [SerializeField] private float collectionRangeV2 = V2_Constants.COLLECTION_RANGE_V2;
    [SerializeField] private float collectionSpeedV2 = V2_Constants.COLLECTION_SPEED_V2;
    [SerializeField] private int maxCarryAmountV2 = 50;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogsV2 = false;

    // Estado
    private int currentCarriedAmountV2 = 0;
    private Transform currentResourceTargetV2;
    private float collectionProgressV2 = 0f;
    private bool isCollectingV2 = false;

    // ==================== REFERENCIAS ====================
    private V2_Unit ownerUnitV2;
    private V2_ResourceManager resourceManagerV2;

    // ==================== PROPIEDADES ====================
    public int CurrentCarriedAmountV2 => currentCarriedAmountV2;
    public int MaxCarryAmountV2 => maxCarryAmountV2;
    public float CarryPercentageV2 => maxCarryAmountV2 > 0 ? (currentCarriedAmountV2 / (float)maxCarryAmountV2) * 100f : 0f;
    public bool IsCollectingV2 => isCollectingV2;

    // ==================== EVENTOS ====================
    public event System.Action<int> OnResourceCollectedV2;
    public event System.Action OnInventoryFullV2;
    public event System.Action OnCollectionStartedV2;
    public event System.Action OnCollectionStoppedV2;

    // ==================== UNITY LIFECYCLE ====================
    private void AwakeInitializeV2()
    {
        ownerUnitV2 = GetComponent<V2_Unit>();
        resourceManagerV2 = FindObjectOfType<V2_ResourceManager>();

        if (ownerUnitV2 == null)
            LogDebugV2("Owner unit not found!", LogType.Error);

        LogDebugV2("Collector initialized");
    }

    private void UpdateCollectionV2()
    {
        if (!isCollectingV2 || currentResourceTargetV2 == null)
            return;

        // Verificar distancia
        float distanceToResourceV2 = Vector3.Distance(transform.position, currentResourceTargetV2.position);

        if (distanceToResourceV2 > collectionRangeV2)
        {
            LogDebugV2($"Resource out of range: {distanceToResourceV2}");
            StopCollectingV2();
            return;
        }

        // Recolectar
        collectionProgressV2 += Time.deltaTime * collectionSpeedV2;

        if (collectionProgressV2 >= 1f)
        {
            CollectOneUnitV2();
            collectionProgressV2 = 0f;

            // Verificar si inventario está lleno
            if (currentCarriedAmountV2 >= maxCarryAmountV2)
            {
                LogDebugV2("Inventory full!");
                OnInventoryFullV2?.Invoke();
                StopCollectingV2();
            }
        }
    }

    private void Awake()
    {
        AwakeInitializeV2();
    }

    private void Update()
    {
        UpdateCollectionV2();
    }

    // ==================== PUBLIC METHODS ====================

    /// <summary>
    /// Inicia la recolección de un recurso
    /// </summary>
    public void StartCollectingResourceV2(Transform resourceTransformV2)
    {
        if (resourceTransformV2 == null)
            return;

        if (currentCarriedAmountV2 >= maxCarryAmountV2)
        {
            LogDebugV2("Inventory full, cannot collect more");
            return;
        }

        currentResourceTargetV2 = resourceTransformV2;
        isCollectingV2 = true;
        collectionProgressV2 = 0f;

        LogDebugV2($"Started collecting from: {resourceTransformV2.gameObject.name}");
        OnCollectionStartedV2?.Invoke();
    }

    /// <summary>
    /// Recolecta directamente de un recurso (usado cuando ya está en rango)
    /// </summary>
    public void CollectFromResourceV2(Transform resourceTransformV2)
    {
        if (resourceTransformV2 == null)
            return;

        StartCollectingResourceV2(resourceTransformV2);
    }

    /// <summary>
    /// Detiene la recolección
    /// </summary>
    public void StopCollectingV2()
    {
        if (!isCollectingV2)
            return;

        isCollectingV2 = false;
        collectionProgressV2 = 0f;
        currentResourceTargetV2 = null;

        LogDebugV2("Stopped collecting");
        OnCollectionStoppedV2?.Invoke();
    }

    /// <summary>
    /// Obtiene la cantidad de recursos que carga
    /// </summary>
    public int GetCarriedAmountV2()
    {
        return currentCarriedAmountV2;
    }

    /// <summary>
    /// Descarga todos los recursos recolectados
    /// </summary>
    public int DropAllResourcesV2()
    {
        int droppedAmountV2 = currentCarriedAmountV2;
        currentCarriedAmountV2 = 0;

        LogDebugV2($"Dropped {droppedAmountV2} resources");
        OnResourceCollectedV2?.Invoke(0); // Notificar cambio

        return droppedAmountV2;
    }

    /// <summary>
    /// Limpia el inventario
    /// </summary>
    public void ClearInventoryV2()
    {
        currentCarriedAmountV2 = 0;
        LogDebugV2("Inventory cleared");
    }

    // ==================== PRIVATE METHODS ====================

    /// <summary>
    /// Recolecta una unidad de recurso
    /// </summary>
    private void CollectOneUnitV2()
    {
        if (currentResourceTargetV2 == null)
            return;

        // Obtener componente del recurso
        V2_Resource resourceV2 = currentResourceTargetV2.GetComponent<V2_Resource>();

        if (resourceV2 != null)
        {
            bool successV2 = resourceV2.TakeResourceV2(1);

            if (successV2)
            {
                currentCarriedAmountV2++;
                OnResourceCollectedV2?.Invoke(currentCarriedAmountV2);
                LogDebugV2($"Collected resource. Total: {currentCarriedAmountV2}/{maxCarryAmountV2}");
            }
            else
            {
                // Recurso agotado
                LogDebugV2("Resource depleted");
                StopCollectingV2();
            }
        }
    }

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
                Debug.LogWarning($"[V2_Collector - {gameObject.name}] {messageV2}");
                break;
            case LogType.Error:
                Debug.LogError($"[V2_Collector - {gameObject.name}] {messageV2}");
                break;
            default:
                Debug.Log($"[V2_Collector - {gameObject.name}] {messageV2}");
                break;
        }
    }
}