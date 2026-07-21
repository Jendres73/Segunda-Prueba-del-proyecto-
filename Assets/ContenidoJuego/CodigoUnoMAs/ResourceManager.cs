using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public GameObject winPanel;
    public Button nextLevelButton;
    public string nextSceneName = "NombreDeTuSiguienteEscena";

    private int totalResourcesCollected = 0;
    public int targetScore = 100;

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(LoadNextLevel);
        UpdateUI();
    }

    // ESTA ES LA FUNCIÓN QUE DEBE EXISTIR
    public void AddCollectedResources(int amount)
    {
        totalResourcesCollected += amount;
        UpdateUI();

        if (totalResourcesCollected >= targetScore)
        {
            if (winPanel != null) winPanel.SetActive(true);
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Recursos: " + totalResourcesCollected + " / " + targetScore;
        }
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}