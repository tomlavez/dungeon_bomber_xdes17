using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelEditor levelEditor;

    [Header("UI Elements")]
    [SerializeField] private InputField fileNameInput;
    [SerializeField] private Text feedbackText;

    // Nome padrão se nenhum for fornecido
    private const string DEFAULT_LEVEL_NAME = "my_level";

    void Start()
    {
        // Definir texto padrão no InputField
        if (fileNameInput != null)
        {
            fileNameInput.text = DEFAULT_LEVEL_NAME;
        }
    }

    /// <summary>
    /// Salva a fase usando o nome do InputField.
    /// </summary>
    public void SaveLevelWithInput()
    {
        if (levelEditor == null)
        {
            ShowFeedback("Error: LevelEditor not assigned!");
            return;
        }

        string fileName = GetFileName();
        levelEditor.SaveLevel(fileName);
    }

    /// <summary>
    /// Carrega a fase usando o nome do InputField.
    /// </summary>
    public void LoadLevelWithInput()
    {
        if (levelEditor == null)
        {
            ShowFeedback("Error: LevelEditor not assigned!");
            return;
        }

        string fileName = GetFileName();
        levelEditor.LoadLevel(fileName);
    }

    /// <summary>
    /// Ativa modo de edição.
    /// </summary>
    public void EnableEditMode()
    {
        if (levelEditor != null)
            levelEditor.EnableEditMode();
    }

    /// <summary>
    /// Desativa modo de edição e inicia o jogo.
    /// </summary>
    public void DisableEditMode()
    {
        if (levelEditor != null)
            levelEditor.DisableEditMode();
    }

    /// <summary>
    /// Limpa todos os objetos da fase.
    /// </summary>
    public void ClearLevel()
    {
        if (levelEditor != null)
            levelEditor.ClearAllObjects();
    }

    /// <summary>
    /// Seleciona tipo de objeto a ser colocado.
    /// </summary>
    public void SelectCoin() => levelEditor?.SelectObjectType("Coin");
    public void SelectChest() => levelEditor?.SelectObjectType("Chest");
    public void SelectDestructibleBlock() => levelEditor?.SelectObjectType("DestructibleBlock");
    public void SelectDestructibleBlockReinforced() => levelEditor?.SelectObjectType("DestructibleBlockReinforced");
    public void SelectSpike() => levelEditor?.SelectObjectType("Spike");

    // ====== MÉTODOS PRIVADOS ======

    private string GetFileName()
    {
        if (fileNameInput == null || string.IsNullOrEmpty(fileNameInput.text))
        {
            return DEFAULT_LEVEL_NAME;
        }

        // Sanitizar nome do arquivo (remover caracteres inválidos)
        string fileName = fileNameInput.text.Trim();
        fileName = SanitizeFileName(fileName);

        return string.IsNullOrEmpty(fileName) ? DEFAULT_LEVEL_NAME : fileName;
    }

    private string SanitizeFileName(string fileName)
    {
        // Remover caracteres inválidos para nomes de arquivo
        char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c.ToString(), "");
        }

        // Remover espaços e substituir por underscore
        fileName = fileName.Replace(" ", "_");

        return fileName;
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        Debug.Log(message);
    }
}
