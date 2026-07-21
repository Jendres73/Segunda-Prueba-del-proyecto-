using UnityEngine;
[ExecuteInEditMode]

public class HealthBarUI : MonoBehaviour
{
    public SpriteRenderer barRenderer;
    public Sprite[] healthSprites; // Arrastra tus 31 imágenes aquí
    public HealthComponent healthComp; // Arrastra tu unidad aquí

    void Update()
    {
        // Evitamos errores si no hay referencia
        if (healthComp == null) return;

        // Calculamos porcentaje (0 a 1)
        float percent = healthComp.currentHealth / healthComp.maxHealth;
        
        // Convertimos a índice (0 a 30)
        int index = Mathf.Clamp(Mathf.FloorToInt(percent * 30), 0, 30);

        // Cambiamos el sprite
        if (barRenderer.sprite != healthSprites[index])
        {
            barRenderer.sprite = healthSprites[index];
        }
    }
}