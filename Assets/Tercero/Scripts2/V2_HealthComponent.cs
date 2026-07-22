using UnityEngine;

/// <summary>
/// V2_HealthComponent.cs - Sistema de salud para unidades
/// Maneja daño, curación y eventos de muerte
/// </summary>
public class V2_HealthComponent : MonoBehaviour
{
    // ==================== VARIABLES ====================
    private float currentHealthV2;
    private float maxHealthV2;
    private bool isAliveV2 = true;

    [SerializeField] private bool enableDebugLogsV2 = false;

    // ==================== EVENTOS ====================
    public event System.Action<float> OnHealthChangedV2;
    public event System.Action OnDeathV2;
    public event System.Action<float> OnDamageTakenV2;
    public event System.Action<float> OnHealedV2;

    // ==================== PROPIEDADES ====================
    public float CurrentHealthV2 => currentHealthV2;
    public float MaxHealthV2 => maxHealthV2;
    public float HealthPercentageV2 => maxHealthV2 > 0 ? (currentHealthV2 / maxHealthV2) * 100f : 0f;
    public bool IsAliveV2 => isAliveV2;

    // ==================== PUBLIC METHODS ====================

    /// <summary>
    /// Inicializa el componente de salud
    /// </summary>
    public void InitializeHealthV2(float maxHealthValueV2)
    {
        maxHealthV2 = maxHealthValueV2;
        currentHealthV2 = maxHealthV2;
        isAliveV2 = true;
        OnHealthChangedV2?.Invoke(currentHealthV2);
        LogDebugV2($"Health initialized: {currentHealthV2}/{maxHealthV2}");
    }

    /// <summary>
    /// Causa daño a la unidad
    /// </summary>
    public void TakeDamageV2(float damageAmountV2)
    {
        if (!isAliveV2 || damageAmountV2 <= 0)
            return;

        currentHealthV2 -= damageAmountV2;
        currentHealthV2 = Mathf.Max(currentHealthV2, 0);

        LogDebugV2($"Took {damageAmountV2} damage. Health: {currentHealthV2}/{maxHealthV2}");
        OnDamageTakenV2?.Invoke(damageAmountV2);
        OnHealthChangedV2?.Invoke(currentHealthV2);

        // Verificar si murió
        if (currentHealthV2 <= 0)
        {
            DieV2();
        }
    }

    /// <summary>
    /// Cura a la unidad
    /// </summary>
    public void HealV2(float healAmountV2)
    {
        if (!isAliveV2 || healAmountV2 <= 0)
            return;

        currentHealthV2 += healAmountV2;
        currentHealthV2 = Mathf.Min(currentHealthV2, maxHealthV2);

        LogDebugV2($"Healed {healAmountV2}. Health: {currentHealthV2}/{maxHealthV2}");
        OnHealedV2?.Invoke(healAmountV2);
        OnHealthChangedV2?.Invoke(currentHealthV2);
    }

    /// <summary>
    /// Restaura salud completa
    /// </summary>
    public void FullHealV2()
    {
        HealV2(maxHealthV2 - currentHealthV2);
    }

    /// <summary>
    /// Obtiene la salud actual
    /// </summary>
    public float GetCurrentHealthV2()
    {
        return currentHealthV2;
    }

    /// <summary>
    /// Obtiene la salud máxima
    /// </summary>
    public float GetMaxHealthV2()
    {
        return maxHealthV2;
    }

    /// <summary>
    /// Verifica si la unidad está viva
    /// </summary>
    public bool IsAliveStateV2()
    {
        return isAliveV2;
    }

    /// <summary>
    /// Establece la salud a un valor específico
    /// </summary>
    public void SetHealthV2(float healthValueV2)
    {
        currentHealthV2 = Mathf.Clamp(healthValueV2, 0, maxHealthV2);
        OnHealthChangedV2?.Invoke(currentHealthV2);

        if (currentHealthV2 <= 0 && isAliveV2)
        {
            DieV2();
        }
    }

    // ==================== PRIVATE METHODS ====================

    /// <summary>
    /// Maneja la muerte de la unidad
    /// </summary>
    private void DieV2()
    {
        if (!isAliveV2)
            return;

        isAliveV2 = false;
        LogDebugV2("Unit died");
        OnDeathV2?.Invoke();
    }

    /// <summary>
    /// Log de debug
    /// </summary>
    private void LogDebugV2(string messageV2)
    {
        if (!enableDebugLogsV2)
            return;

        Debug.Log($"[V2_HealthComponent - {gameObject.name}] {messageV2}");
    }
}