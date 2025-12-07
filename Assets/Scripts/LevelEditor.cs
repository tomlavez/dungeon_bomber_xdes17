using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Script principal do Level Editor.
/// Gerencia modo de edição, colocação/remoção de objetos, e integração com save/load.
/// </summary>
public class LevelEditor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavGrid navGrid;
    [SerializeField] private Transform objectsContainer; // Parent para organizar objetos criados

    [Header("Prefabs")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject destructibleBlockPrefab;
    [SerializeField] private GameObject destructibleBlockReinforcedPrefab;
    [SerializeField] private GameObject spikePrefab;

    [Header("UI References")]
    [SerializeField] private GameObject editorUIPanel; // Painel com botões de edição
    [SerializeField] private TextMeshProUGUI statusText; // Texto para mostrar mensagens (TextMeshPro)

    [Header("Game UI Elements to Hide")]
    [SerializeField] private GameObject healthUI; // HealthUI (pai dos corações)
    [SerializeField] private GameObject scoreUI; // ScoreManager GameObject (com TextMeshProUGUI)
    [SerializeField] private GameObject timerUI; // GameTimer GameObject (com TextMeshProUGUI)

    // Estado interno
    private bool isEditMode = false;
    private string selectedObjectType = "";
    private Dictionary<Vector2Int, GameObject> placedObjects; // Rastreia objetos em cada c�lula
    private bool lastPlayModeWasPlayer = false; // Rastreia se o último modo foi Player ou AI

    void Awake()
    {
        placedObjects = new Dictionary<Vector2Int, GameObject>();
    }

    void Start()
    {
        // Iniciar com modo de edição ativado quando vindo do menu principal
        bool startInEditMode = true; // Sempre começa no editor

        if (startInEditMode)
            EnableEditMode();
        else
            DisableEditMode();
    }

    void Update()
    {
        // Atalho de teclado: pressione E para ativar/desativar Edit Mode
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isEditMode)
                DisableEditMode();
            else
                EnableEditMode();
        }

        if (!isEditMode) return;

        // Detectar clique do mouse (New Input System)
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) // Botão esquerdo - colocar objeto
            {
                // Ignorar cliques sobre UI
                if (!IsPointerOverUI())
                {
                    HandleLeftClick();
                }
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame) // Botão direito - remover objeto
            {
                // Ignorar cliques sobre UI
                if (!IsPointerOverUI())
                {
                    HandleRightClick();
                }
            }
        }
    }

    /// <summary>
    /// Ativa o modo de edição.
    /// </summary>
    public void EnableEditMode()
    {
        isEditMode = true;
        Time.timeScale = 0f; // Pausa o jogo

        // Desabilitar AI para evitar pathfinding durante edição
        AIController[] aiControllers = FindObjectsByType<AIController>(FindObjectsSortMode.None);
        foreach (var ai in aiControllers)
        {
            ai.enabled = false;
        }

        if (editorUIPanel != null)
            editorUIPanel.SetActive(true);

        // Ocultar elementos de UI do jogo
        HideGameUI();

        UpdateStatusText("Edit Mode Active - Select an object type");
        Debug.Log("Edit Mode Enabled");
    }

    /// <summary>
    /// Desativa o modo de edição.
    /// </summary>
    public void DisableEditMode()
    {
        isEditMode = false;
        Time.timeScale = 1f; // Resume o jogo

        if (editorUIPanel != null)
            editorUIPanel.SetActive(false);

        // Mostrar elementos de UI do jogo
        ShowGameUI();

        selectedObjectType = "";
        UpdateStatusText("");
        Debug.Log("Edit Mode Disabled");
    }

    /// <summary>
    /// Verifica se o modo de edição está ativo.
    /// </summary>
    public bool IsEditModeActive()
    {
        return isEditMode;
    }

    /// <summary>
    /// Verifica se o último modo de jogo foi Player (true) ou AI (false).
    /// </summary>
    public bool IsPlayerControlMode()
    {
        return lastPlayModeWasPlayer;
    }

    /// <summary>
    /// Restaura o modo de controle do player baseado no último modo escolhido.
    /// </summary>
    public void RestorePlayerControlMode(Player player)
    {
        ConfigurePlayerControl(player, lastPlayModeWasPlayer);
        Debug.Log($"[LevelEditor] Restored control mode: {(lastPlayModeWasPlayer ? "PLAYER" : "AI")}");
    }

    /// <summary>
    /// Desativa o modo de edição e inicia o gameplay com a fase editada.
    /// Use este método no botão "Play" do editor para testar a fase sem recarregar a cena.
    /// </summary>
    public void PlayEditedLevel()
    {
        // Salvar a fase atual antes de começar a jogar
        SaveLevel("_current_level");
        Debug.Log("[LevelEditor] Current level saved before playing");

        // Limpar bombas e explosões ativas
        Bomba[] bombs = FindObjectsByType<Bomba>(FindObjectsSortMode.None);
        foreach (var bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }

        Explosao[] explosions = FindObjectsByType<Explosao>(FindObjectsSortMode.None);
        foreach (var explosion in explosions)
        {
            Destroy(explosion.gameObject);
        }
        Debug.Log($"[LevelEditor] Destroyed {bombs.Length} bombs and {explosions.Length} explosions");

        // Atualizar o NavGrid PRIMEIRO, antes de qualquer outra coisa
        if (navGrid != null)
        {
            navGrid.RefreshGrid();
            Debug.Log("[LevelEditor] NavGrid refreshed before starting gameplay");
        }

        // Resetar posição do jogador para posição inicial
        Player player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
        if (player != null)
        {
            // Reabilitar o GameObject do player
            player.gameObject.SetActive(true);

            player.transform.position = new Vector3(-6.5f, -0.5f, 0f);

            // Configurar para modo AI
            ConfigurePlayerControl(player, false);
            Debug.Log("[LevelEditor] Player reactivated and position reset to start - AI MODE");
        }

        // Marcar que o modo AI foi escolhido
        lastPlayModeWasPlayer = false;

        // Resetar timer
        GameTimer timer = FindFirstObjectByType<GameTimer>();
        if (timer != null)
        {
            timer.ResetTimer();
            Debug.Log("[LevelEditor] Timer reset");
        }

        // Reabilitar AIControllers DEPOIS do grid atualizado
        AIController[] aiControllers = FindObjectsByType<AIController>(FindObjectsSortMode.None);
        foreach (var ai in aiControllers)
        {
            ai.enabled = true;
            Debug.Log("[LevelEditor] AI Controller enabled with updated grid");
        }

        isEditMode = false;
        Time.timeScale = 1f; // Resume o jogo

        if (editorUIPanel != null)
            editorUIPanel.SetActive(false);

        // Mostrar elementos de UI do jogo
        ShowGameUI();

        // Resetar score e preparar jogo
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }

        selectedObjectType = "";
        UpdateStatusText("");
        Debug.Log("[LevelEditor] Playing edited level with AI - Game started without reloading scene");
    }

    /// <summary>
    /// Desativa o modo de edição e inicia o gameplay para o jogador humano (sem IA).
    /// Use este método no botão "Play Player" do editor.
    /// </summary>
    public void PlayEditedLevelAsPlayer()
    {
        // Salvar a fase atual antes de começar a jogar
        SaveLevel("_current_level");
        Debug.Log("[LevelEditor] Current level saved before playing");

        // Limpar bombas e explosões ativas
        Bomba[] bombs = FindObjectsByType<Bomba>(FindObjectsSortMode.None);
        foreach (var bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }

        Explosao[] explosions = FindObjectsByType<Explosao>(FindObjectsSortMode.None);
        foreach (var explosion in explosions)
        {
            Destroy(explosion.gameObject);
        }
        Debug.Log($"[LevelEditor] Destroyed {bombs.Length} bombs and {explosions.Length} explosions");

        // Atualizar o NavGrid PRIMEIRO, antes de qualquer outra coisa
        if (navGrid != null)
        {
            navGrid.RefreshGrid();
            Debug.Log("[LevelEditor] NavGrid refreshed before starting gameplay");
        }

        // Resetar posição do jogador para posição inicial
        Player player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
        if (player != null)
        {
            // Reabilitar o GameObject do player
            player.gameObject.SetActive(true);

            player.transform.position = new Vector3(-6.5f, -0.5f, 0f);

            // Configurar para modo Player
            ConfigurePlayerControl(player, true);
            Debug.Log("[LevelEditor] Player reactivated and position reset to start - PLAYER MODE");
        }

        // Marcar que o modo Player foi escolhido
        lastPlayModeWasPlayer = true;

        // Resetar timer
        GameTimer timer = FindFirstObjectByType<GameTimer>();
        if (timer != null)
        {
            timer.ResetTimer();
            Debug.Log("[LevelEditor] Timer reset");
        }

        isEditMode = false;
        Time.timeScale = 1f; // Resume o jogo

        if (editorUIPanel != null)
            editorUIPanel.SetActive(false);

        // Mostrar elementos de UI do jogo
        ShowGameUI();

        // Resetar score e preparar jogo
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }

        selectedObjectType = "";
        UpdateStatusText("");
        Debug.Log("[LevelEditor] Playing edited level as PLAYER - Game started without reloading scene");
    }

    /// <summary>
    /// Configura o controle do player para modo humano ou IA.
    /// </summary>
    private void ConfigurePlayerControl(Player player, bool isPlayerMode)
    {
        PlayerKeyboardController humanController = player.GetComponent<PlayerKeyboardController>();
        AIController aiController = player.GetComponent<AIController>();
        PlayerInput playerInput = player.GetComponent<PlayerInput>();

        if (isPlayerMode)
        {
            // Modo Player: Habilitar controle humano
            if (humanController != null)
            {
                humanController.enabled = true;
                Debug.Log("[LevelEditor] Human controller enabled");
            }

            if (playerInput != null)
            {
                playerInput.enabled = true;
                Debug.Log("[LevelEditor] PlayerInput enabled");
            }

            if (aiController != null)
            {
                aiController.enabled = false;
                Debug.Log("[LevelEditor] AI controller disabled");
            }
        }
        else
        {
            // Modo AI: Desabilitar controle humano
            if (humanController != null)
            {
                humanController.enabled = false;
                Debug.Log("[LevelEditor] Human controller disabled");
            }

            if (playerInput != null)
            {
                playerInput.enabled = false;
                Debug.Log("[LevelEditor] PlayerInput disabled");
            }

            if (aiController != null)
            {
                aiController.enabled = true;
                Debug.Log("[LevelEditor] AI controller enabled");
            }
        }
    }

    /// <summary>
    /// Seleciona o tipo de objeto a ser colocado.
    /// Tipos válidos: "Coin", "Chest", "DestructibleBlock", "DestructibleBlockReinforced", "Spike"
    /// </summary>
    public void SelectObjectType(string type)
    {
        selectedObjectType = type;
        UpdateStatusText($"Selected: {type} - Click on grid to place");
        Debug.Log($"Object type selected: {type}");
    }

    /// <summary>
    /// Ativa o modo de remoção. Clique nos objetos para removê-los.
    /// Use este método no botão "Remove Object".
    /// </summary>
    public void SelectRemoveMode()
    {
        selectedObjectType = "REMOVE";
        UpdateStatusText("Remove Mode - Click on objects to remove them");
        Debug.Log("[LevelEditor] Remove mode activated");
    }

    /// <summary>
    /// Remove todos os objetos da fase.
    /// </summary>
    public void ClearAllObjects()
    {
        if (objectsContainer == null)
        {
            Debug.LogError("Objects container is not assigned!");
            return;
        }

        // Destruir todos os filhos do container
        foreach (Transform child in objectsContainer)
        {
            Destroy(child.gameObject);
        }

        // Limpar dicion�rio de objetos
        placedObjects.Clear();

        UpdateStatusText("All objects cleared");
        Debug.Log("Level cleared");
    }

    /// <summary>
    /// Salva a fase atual em um arquivo JSON.
    /// </summary>
    public void SaveLevel(string fileName)
    {
        if (navGrid == null || objectsContainer == null)
        {
            Debug.LogError("NavGrid or Objects Container not assigned!");
            UpdateStatusText("Error: Missing references");
            return;
        }

        string result = LevelSaver.SaveLevelToFile(fileName, navGrid, objectsContainer);
        UpdateStatusText(result);
        Debug.Log(result);
    }

    /// <summary>
    /// Carrega uma fase de um arquivo JSON.
    /// </summary>
    public void LoadLevel(string fileName)
    {
        if (navGrid == null || objectsContainer == null)
        {
            Debug.LogError("NavGrid or Objects Container not assigned!");
            UpdateStatusText("Error: Missing references");
            return;
        }

        // Limpar objetos atuais antes de carregar
        ClearAllObjects();

        string result = LevelLoader.LoadLevelFromFile(
            fileName,
            navGrid,
            objectsContainer,
            coinPrefab,
            chestPrefab,
            destructibleBlockPrefab,
            destructibleBlockReinforcedPrefab,
            spikePrefab
        );

        // Reconstruir dicion�rio de objetos ap�s carregar
        RebuildObjectsDictionary();

        UpdateStatusText(result);
        Debug.Log(result);
    }

    // ====== MÉTODOS PRIVADOS ======

    /// <summary>
    /// Oculta elementos de UI do jogo durante edição.
    /// </summary>
    private void HideGameUI()
    {
        if (healthUI != null)
        {
            healthUI.SetActive(false);
            Debug.Log("[LevelEditor] HealthUI hidden");
        }

        if (scoreUI != null)
        {
            scoreUI.SetActive(false);
            Debug.Log("[LevelEditor] ScoreUI hidden");
        }

        if (timerUI != null)
        {
            timerUI.SetActive(false);
            Debug.Log("[LevelEditor] TimerUI hidden");
        }
    }

    /// <summary>
    /// Mostra elementos de UI do jogo ao sair do modo de edição.
    /// </summary>
    private void ShowGameUI()
    {
        if (healthUI != null)
        {
            healthUI.SetActive(true);
            Debug.Log("[LevelEditor] HealthUI shown");
        }

        if (scoreUI != null)
        {
            scoreUI.SetActive(true);
            Debug.Log("[LevelEditor] ScoreUI shown");
        }

        if (timerUI != null)
        {
            timerUI.SetActive(true);
            Debug.Log("[LevelEditor] TimerUI shown");
        }
    }

    /// <summary>
    /// Verifica se o mouse está sobre algum botão de UI (não bloqueia cliques em painéis vazios).
    /// </summary>
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        // Para New Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = mousePos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Verificar se algum dos elementos é um botão ou componente interativo
        foreach (var result in results)
        {
            // Bloquear apenas se for um Button ou outro componente interativo
            if (result.gameObject.GetComponent<Button>() != null ||
                result.gameObject.GetComponent<UnityEngine.UI.Slider>() != null ||
                result.gameObject.GetComponent<UnityEngine.UI.Toggle>() != null ||
                result.gameObject.GetComponent<UnityEngine.UI.Dropdown>() != null ||
                result.gameObject.GetComponent<UnityEngine.UI.InputField>() != null ||
                result.gameObject.GetComponent<TMPro.TMP_InputField>() != null)
            {
                Debug.Log($"[LevelEditor] Click ignored - pointer is over interactive UI: {result.gameObject.name}");
                return true;
            }
        }

        // Não está sobre nenhum elemento interativo, permite o clique no grid
        return false;
    }

    private void HandleLeftClick()
    {
        Debug.Log("[LevelEditor] HandleLeftClick called!");

        if (string.IsNullOrEmpty(selectedObjectType))
        {
            UpdateStatusText("No object type selected!");
            Debug.LogWarning("[LevelEditor] No object type selected!");
            return;
        }

        // Converter posição do mouse para coordenadas do mundo
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // LOG: Posição do mouse
        Debug.Log($"[LevelEditor] Mouse Screen: {mousePos} → World: {worldPos}");

        // Ajustar worldPos para compensar offset (alinhar com bordas das células)
        // GridToWorld retorna o centro da célula, mas queremos detectar pela borda
        Vector2 adjustedWorldPos = worldPos + new Vector2(navGrid.cellSize * 0.5f, -navGrid.cellSize * 0.5f);
        Debug.Log($"[LevelEditor] Adjusted World Pos: {adjustedWorldPos}");

        // Converter para coordenadas do grid
        Node node = navGrid.WorldToNode(adjustedWorldPos);
        Vector2Int gridPos = new Vector2Int(node.x, node.y);

        // LOG: Conversão para grid
        Debug.Log($"[LevelEditor] Grid Position: ({gridPos.x}, {gridPos.y}) | Node World Pos: {node.worldPos}");

        // Validar limites do grid
        if (!IsValidGridPosition(gridPos))
        {
            UpdateStatusText($"Invalid position: ({gridPos.x}, {gridPos.y}) - Out of bounds");
            Debug.LogWarning($"[LevelEditor] Position out of bounds! Grid: {navGrid.width}x{navGrid.height}");
            return;
        }

        // Se está no modo de remoção, remover objeto
        if (selectedObjectType == "REMOVE")
        {
            RemoveObject(gridPos);
            return;
        }

        // Verificar se célula já está ocupada
        if (placedObjects.ContainsKey(gridPos))
        {
            UpdateStatusText($"Cell occupied at ({gridPos.x}, {gridPos.y})");
            Debug.LogWarning($"[LevelEditor] Cell ({gridPos.x}, {gridPos.y}) is already occupied!");
            return;
        }

        // Colocar objeto
        PlaceObject(selectedObjectType, gridPos);
    }

    private void HandleRightClick()
    {
        // Converter posição do mouse para coordenadas do mundo
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Ajustar worldPos para compensar offset (alinhar com bordas das células)
        Vector2 adjustedWorldPos = worldPos + new Vector2(navGrid.cellSize * 0.5f, -navGrid.cellSize * 0.5f);

        // Converter para coordenadas do grid
        Node node = navGrid.WorldToNode(adjustedWorldPos);
        Vector2Int gridPos = new Vector2Int(node.x, node.y);

        // Validar limites do grid
        if (!IsValidGridPosition(gridPos))
        {
            return;
        }

        // Remover objeto se existir
        RemoveObject(gridPos);
    }

    private void PlaceObject(string type, Vector2Int gridPos)
    {
        GameObject prefab = GetPrefabByType(type);
        if (prefab == null)
        {
            UpdateStatusText($"Error: Prefab not found for {type}");
            Debug.LogError($"Prefab for {type} is not assigned!");
            return;
        }

        // Calcular posição mundial
        Vector2 worldPos = navGrid.GridToWorld(gridPos.x, gridPos.y);

        // LOG: Posicionamento do objeto
        Debug.Log($"[LevelEditor] Placing {type} at Grid({gridPos.x}, {gridPos.y}) → World({worldPos.x:F2}, {worldPos.y:F2})");

        // Instanciar objeto
        GameObject newObject = Instantiate(prefab, worldPos, Quaternion.identity, objectsContainer);

        // Configurar parâmetros baseado no tipo
        ConfigureObjectParameters(newObject, type);

        // Adicionar ao dicionário
        placedObjects[gridPos] = newObject;

        UpdateStatusText($"{type} placed at ({gridPos.x}, {gridPos.y})");
        Debug.Log($"[LevelEditor] ✓ Successfully placed {type} at grid ({gridPos.x}, {gridPos.y})");
    }

    private void RemoveObject(Vector2Int gridPos)
    {
        if (!placedObjects.ContainsKey(gridPos))
        {
            UpdateStatusText($"No object at ({gridPos.x}, {gridPos.y})");
            return;
        }

        GameObject obj = placedObjects[gridPos];
        string objType = GetObjectType(obj);

        Destroy(obj);
        placedObjects.Remove(gridPos);

        UpdateStatusText($"{objType} removed from ({gridPos.x}, {gridPos.y})");
        Debug.Log($"Removed {objType} from grid ({gridPos.x}, {gridPos.y})");
    }

    private bool IsValidGridPosition(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < navGrid.width &&
               gridPos.y >= 0 && gridPos.y < navGrid.height;
    }

    private GameObject GetPrefabByType(string type)
    {
        switch (type)
        {
            case "Coin": return coinPrefab;
            case "Chest": return chestPrefab;
            case "DestructibleBlock": return destructibleBlockPrefab;
            case "DestructibleBlockReinforced": return destructibleBlockReinforcedPrefab;
            case "Spike": return spikePrefab;
            default: return null;
        }
    }

    private void ConfigureObjectParameters(GameObject obj, string type)
    {
        switch (type)
        {
            case "Coin":
                // Coin j� tem valor padr�o de 10 no prefab
                break;

            case "Chest":
                // Chest j� tem valor padr�o de 100 no prefab
                break;

            case "DestructibleBlock":
                // Setar health = 1
                var block = obj.GetComponent<DestructibleBlockReinforced>();
                if (block != null)
                {
                    SetPrivateField(block, "health", 1);
                }
                break;

            case "DestructibleBlockReinforced":
                // Setar health = 2
                var blockReinforced = obj.GetComponent<DestructibleBlockReinforced>();
                if (blockReinforced != null)
                {
                    SetPrivateField(blockReinforced, "health", 2);
                }
                break;

            case "Spike":
                // Spike n�o precisa de configura��o adicional
                break;
        }
    }

    private void SetPrivateField(Component component, string fieldName, object value)
    {
        var field = component.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(component, value);
        }
        else
        {
            Debug.LogWarning($"Field {fieldName} not found in {component.GetType().Name}");
        }
    }

    private string GetObjectType(GameObject obj)
    {
        if (obj.GetComponent<Coin>() != null) return "Coin";
        if (obj.GetComponent<Chest>() != null) return "Chest";
        if (obj.GetComponent<Spike>() != null) return "Spike";
        if (obj.GetComponent<DestructibleBlockReinforced>() != null)
        {
            // Diferenciar entre bloco normal e refor�ado pelo health
            var block = obj.GetComponent<DestructibleBlockReinforced>();
            return block.GetHealth() == 1 ? "DestructibleBlock" : "DestructibleBlockReinforced";
        }
        return "Unknown";
    }

    private void RebuildObjectsDictionary()
    {
        placedObjects.Clear();

        if (objectsContainer == null) return;

        foreach (Transform child in objectsContainer)
        {
            Node node = navGrid.WorldToNode(child.position);
            Vector2Int gridPos = new Vector2Int(node.x, node.y);
            if (IsValidGridPosition(gridPos))
            {
                placedObjects[gridPos] = child.gameObject;
            }
        }

        Debug.Log($"Rebuilt objects dictionary: {placedObjects.Count} objects");
    }

    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
