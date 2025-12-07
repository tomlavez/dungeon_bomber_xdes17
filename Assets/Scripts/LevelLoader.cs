using UnityEngine;
using System.IO;
using System.Reflection;

/// <summary>
/// Script para carregar fases de arquivos JSON.
/// Deserializa o JSON e instancia todos os objetos com seus parâmetros corretos.
/// </summary>
public static class LevelLoader
{
    /// <summary>
    /// Carrega uma fase de um arquivo JSON.
    /// </summary>
    /// <param name="fileName">Nome do arquivo (sem extens�o)</param>
    /// <param name="navGrid">Refer�ncia ao NavGrid para convers�o de coordenadas e regenera��o</param>
    /// <param name="objectsContainer">Transform onde os objetos ser�o instanciados</param>
    /// <param name="coinPrefab">Prefab da moeda</param>
    /// <param name="chestPrefab">Prefab do ba�</param>
    /// <param name="destructibleBlockPrefab">Prefab do bloco destru�vel normal</param>
    /// <param name="destructibleBlockReinforcedPrefab">Prefab do bloco destru�vel refor�ado</param>
    /// <param name="spikePrefab">Prefab da armadilha</param>
    /// <returns>Mensagem de sucesso ou erro</returns>
    public static string LoadLevelFromFile(
        string fileName,
        NavGrid navGrid,
        Transform objectsContainer,
        GameObject coinPrefab,
        GameObject chestPrefab,
        GameObject destructibleBlockPrefab,
        GameObject destructibleBlockReinforcedPrefab,
        GameObject spikePrefab)
    {
        // Valida��es
        if (navGrid == null)
        {
            Debug.LogError("NavGrid is null!");
            return "Error: NavGrid not found";
        }

        if (objectsContainer == null)
        {
            Debug.LogError("Objects container is null!");
            return "Error: Objects container not found";
        }

        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("File name is empty!");
            return "Error: Invalid file name";
        }

        try
        {
            // Caminho completo do arquivo
            string directoryPath = Path.Combine(Application.persistentDataPath, "levels");
            string filePath = Path.Combine(directoryPath, fileName + ".json");

            // Verificar se arquivo existe
            if (!File.Exists(filePath))
            {
                string errorMsg = $"File not found: {fileName}";
                Debug.LogError($"{errorMsg} at {filePath}");
                return errorMsg;
            }

            // Ler arquivo JSON
            string json = File.ReadAllText(filePath);

            // Deserializar
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            if (levelData == null)
            {
                Debug.LogError("Failed to deserialize level data!");
                return "Error: Invalid JSON format";
            }

            // Validar estrutura
            if (levelData.objects == null)
            {
                Debug.LogError("Level data has no objects list!");
                return "Error: Invalid level structure";
            }

            // Limpar objetos existentes
            ClearContainer(objectsContainer);

            // Instanciar objetos
            int successCount = 0;
            int failCount = 0;

            foreach (LevelObject levelObj in levelData.objects)
            {
                bool success = InstantiateObject(
                    levelObj,
                    navGrid,
                    objectsContainer,
                    coinPrefab,
                    chestPrefab,
                    destructibleBlockPrefab,
                    destructibleBlockReinforcedPrefab,
                    spikePrefab
                );

                if (success)
                    successCount++;
                else
                    failCount++;
            }

            // Regenerar grid para detectar novos obst�culos
            navGrid.RefreshGrid();

            string resultMessage = $"Level loaded: {fileName} ({successCount} objects)";
            if (failCount > 0)
            {
                resultMessage += $" - {failCount} failed";
            }

            Debug.Log(resultMessage);
            return resultMessage;
        }
        catch (System.Exception ex)
        {
            string errorMessage = $"Failed to load level: {ex.Message}";
            Debug.LogError(errorMessage);
            return errorMessage;
        }
    }

    /// <summary>
    /// Limpa todos os objetos do container.
    /// </summary>
    private static void ClearContainer(Transform container)
    {
        // Criar lista tempor�ria para evitar modifica��o durante itera��o
        int childCount = container.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(container.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Instancia um objeto individual baseado em seus dados.
    /// </summary>
    private static bool InstantiateObject(
        LevelObject levelObj,
        NavGrid navGrid,
        Transform container,
        GameObject coinPrefab,
        GameObject chestPrefab,
        GameObject destructibleBlockPrefab,
        GameObject destructibleBlockReinforcedPrefab,
        GameObject spikePrefab)
    {
        // Determinar qual prefab usar
        GameObject prefab = GetPrefabByType(
            levelObj.type,
            coinPrefab,
            chestPrefab,
            destructibleBlockPrefab,
            destructibleBlockReinforcedPrefab,
            spikePrefab
        );

        if (prefab == null)
        {
            Debug.LogWarning($"Unknown object type: {levelObj.type}");
            return false;
        }

        // Calcular posi��o mundial
        Vector2 worldPos = navGrid.GridToWorld(levelObj.gridX, levelObj.gridY);

        // Instanciar objeto
        GameObject instance = GameObject.Instantiate(prefab, worldPos, Quaternion.identity, container);

        // Configurar par�metros
        ConfigureObjectParameters(instance, levelObj);

        return true;
    }

    /// <summary>
    /// Retorna o prefab correto baseado no tipo de objeto.
    /// </summary>
    private static GameObject GetPrefabByType(
        string type,
        GameObject coinPrefab,
        GameObject chestPrefab,
        GameObject destructibleBlockPrefab,
        GameObject destructibleBlockReinforcedPrefab,
        GameObject spikePrefab)
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

    /// <summary>
    /// Configura os par�metros de um objeto instanciado usando reflection.
    /// </summary>
    private static void ConfigureObjectParameters(GameObject obj, LevelObject levelObj)
    {
        switch (levelObj.type)
        {
            case "Coin":
                SetSerializedField(obj, "Coin", "coinValue", levelObj.value);
                break;

            case "Chest":
                SetSerializedField(obj, "Chest", "chestValue", levelObj.value);
                break;

            case "DestructibleBlock":
            case "DestructibleBlockReinforced":
                SetSerializedField(obj, "DestructibleBlockReinforced", "health", levelObj.health);
                break;

            case "Spike":
                // Spike n�o tem par�metros configur�veis
                break;
        }
    }

    /// <summary>
    /// Seta um campo serializado privado usando reflection.
    /// </summary>
    private static void SetSerializedField(GameObject obj, string componentTypeName, string fieldName, object value)
    {
        Component component = obj.GetComponent(componentTypeName);
        if (component == null)
        {
            Debug.LogWarning($"Component {componentTypeName} not found on {obj.name}");
            return;
        }

        FieldInfo field = component.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (field != null)
        {
            field.SetValue(component, value);
            Debug.Log($"Set {componentTypeName}.{fieldName} = {value} on {obj.name}");
        }
        else
        {
            Debug.LogWarning($"Field {fieldName} not found in {componentTypeName}");
        }
    }
}

