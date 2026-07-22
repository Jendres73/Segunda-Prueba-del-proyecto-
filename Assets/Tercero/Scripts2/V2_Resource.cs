using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// V2_Resource.cs - Representa un recurso que puede ser recolectado
/// Se añade a GameObjects que representan recursos en el mapa
/// </summary>
public class V2_Resource : MonoBehaviour
{
    // ==================== VARIABLES ====================
    [Header("Resource Settings")]
    [SerializeField] private ResourceType resourceTypeV2 = ResourceType.Wood;
    [SerializeField] private int totalAmountV2 = V2_Constants.MAX_RESOURCE_AMOUNT_V2;
    [SerializeField] private float respawnTimeV2 = V2_Constants.RESOURCE_RESPAWN_TIME_V2;

    [Header("Visual")]
    [SerializeField] private Image resourceAmountDisplayV2; // Imagen de UI para mostrar cantidad
    [SerializeField] private Color fullColorV2 = Color.green;
    [SerializeField] private Color depletedColorV2 = Color.red;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogsV2 = false;

    // Estado
    private int currentAmountV2;
    private bool isDepletedV2 = false;
    private float respawnTimerV2 = 0f;
    private Collider2D colliderComponentV2;

    // ==================== PROPIEDADES ====================
    public ResourceType ResourceTypeV2 => resourceTypeV2;
    public int CurrentAmountV2 => currentAmountV2;
    public int TotalAmountV2 => totalAmountV2;
    public float AmountPercentageV2 => totalAmountV2 > 0 ? (currentAmountV2 / (float)totalAmountV2) * 100f : 0f;
    public bool IsDepletedV2 => isDepletedV2;

    // ==================== EVENTOS ====================
    public event System.Action<int> OnResourceTakenV2;
    public event System.Action OnResourceDepletedV2;
    public event System.Action OnResourceRespawnedV2;

    // ==================== UNITY LIFECYCLE ====================
    private void AwakeInitializeV2()
    {
        colliderComponentV2 = GetComponent<Collider2D>();
        currentAmountV2 = totalAmountV2;

        // Asegurar que tiene el tag correcto
        if (!gameObject.CompareTag(V2_Constants.TAG_RESOURCE_V2))
        {
            gameObject.tag = V2_Constants.TAG_RESOURCE_V2;
        }

        LogDebugV2($"Resource initialized: {resourceTypeV2}, Amount: {currentAmountV2}");
    }

    private void UpdateRespawnV2()
    {
        if (!isDepletedV2)
            return;

        respawnTimerV2 -= Time.deltaTime;

        if (respawnTimerV2 <= 0)
        {
            RespawnResourceV2();
        }
    }

    private void Awake()
    {
        AwakeInitializeV2();
    }

    private void Update()
    {
        UpdateRespawnV2();
    }

    // ==================== PUBLIC METHODS ====================

    /// <summary>
    /// Toma una cantidad de recurso
    /// </summary>
    public bool TakeResourceV2(int amountV2)
    {
        if (isDepletedV2 || amountV2 <= 0)
            return false;

        currentAmountV2 -= amountV2;

        if (currentAmountV2 <= 0)
        {
            currentAmountV2 = 0;
            DepletResourceV2();
        }

        OnResourceTakenV2?.Invoke(currentAmountV2);
        UpdateVisualV2();
        LogDebugV2($"Took {amountV2} resource. Remaining: {currentAmountV2}");

        return true;
    }

    /// <summary>
    /// Toma todo el recurso disponible
    /// </summary>
    public int TakeAllResourceV2()
    {
        int amountTakenV2 = currentAmountV2;
        TakeResourceV2(amountTakenV2);
        return amountTakenV2;
    }

    /// <summary>
    /// Obtiene la cantidad actual de recurso
    /// </summary>
    public int GetCurrentAmountV2()
    {
        return currentAmountV2;
    }

    /// <summary>
    /// Verifica si hay recurso disponible
    /// </summary>
    public bool HasResourceAvailableV2()
    {
        return !isDepletedV2 && currentAmountV2 > 0;
    }

    // ==================== PRIVATE METHODS ====================

    /// <summary>
    /// Agota el recurso
    /// </summary>
    private void DepletResourceV2()
    {
        isDepletedV2 = true;
        respawnTimerV2 = respawnTimeV2;

        // Deshabilitar colisiones
        if (colliderComponentV2 != null)
        {
            colliderComponentV2.enabled = false;
        }

        UpdateVisualV2();
        LogDebugV2("Resource depleted, waiting for respawn");
        OnResourceDepletedV2?.Invoke();
    }

    /// <summary>
    /// Regenera el recurso
    /// </summary>
    private void RespawnResourceV2()
    {
        isDepletedV2 = false;
        currentAmountV2 = totalAmountV2;
        respawnTimerV2 = 0f;

        // Habilitar colisiones
        if (colliderComponentV2 != null)
        {
            colliderComponentV2.enabled = true;
        }

        UpdateVisualV2();
        LogDebugV2("Resource respawned");
        OnResourceRespawnedV2?.Invoke();
    }

    /// <summary>
    /// Actualiza el visual del recurso
    /// </summary>
    private void UpdateVisualV2()
    {
        if (resourceAmountDisplayV2 != null)
        {
            float percentageV2 = AmountPercentageV2 / 100f;
            resourceAmountDisplayV2.fillAmount = percentageV2;

            // Cambiar color según cantidad
            if (isDepletedV2)
            {
                resourceAmountDisplayV2.color = depletedColorV2;
            }
            else if (percentageV2 < 0.3f)
            {
                resourceAmountDisplayV2.color = Color.Lerp(depletedColorV2, fullColorV2, percentageV2 / 0.3f);
            }
            else
            {
                resourceAmountDisplayV2.color = fullColorV2;
            }
        }

        // Cambiar sprite o escala según cantidad
        float scalePercentageV2 = Mathf.Clamp01(AmountPercentageV2 / 100f);
        transform.localScale = Vector3.one * scalePercentageV2;
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
                Debug.LogWarning($"[V2_Resource - {gameObject.name}] {messageV2}");
                break;
            case LogType.Error:
                Debug.LogError($"[V2_Resource - {gameObject.name}] {messageV2}");
                break;
            default:
                Debug.Log($"[V2_Resource - {gameObject.name}] {messageV2}");
                break;
        }
    }
}