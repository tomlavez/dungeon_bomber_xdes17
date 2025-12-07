using UnityEngine;
using System.IO;

/// <summary>
/// Script para serializar a fase atual e salvar em arquivo JSON.
/// Percorre todos os objetos na cena e gera um arquivo JSON com suas informa��es.
/// </summary>
public static class LevelSaver
{
    /// <summary>
    /// Salva a fase atual em um arquivo JSON.
    /// </summary>
    /// <param name="fileName">Nome do arquivo (sem extens�o)</param>
    /// <param name="navGrid">Refer�ncia ao NavGrid para obter dimens�es e convers�o de coordenadas</param>
    /// <param name="objectsContainer">Transform que cont�m todos os objetos da fase</param>
    /// <returns>Mensagem de sucesso ou erro</returns>
    public static string SaveLevelToFile(string fileName, NavGrid navGrid, Transform objectsContainer)
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
            // Criar estrutura LevelData
            LevelData levelData = new LevelData
            {
                gridWidth = navGrid.width,
                gridHeight = navGrid.height,
                cellSize = 1.0f, // Valor fixo baseado na especifica��o
                gridOriginX = navGrid.gridOrigin.x,
                gridOriginY = navGrid.gridOrigin.y
            };

            // Percorrer todos os objetos filhos do container
            int objectCount = 0;
            foreach (Transform child in objectsContainer)
            {
                GameObject obj = child.gameObject;

                // Determinar tipo do objeto
                string objectType = DetermineObjectType(obj);
                if (string.IsNullOrEmpty(objectType))
                {
                    Debug.LogWarning($"Unknown object type for: {obj.name}");
                    continue;
                }

                // Converter posição mundial para coordenadas do grid
                Node node = navGrid.WorldToNode(child.position);
                Vector2Int gridPos = new Vector2Int(node.x, node.y);

                // Validar que está dentro dos limites
                if (gridPos.x < 0 || gridPos.x >= navGrid.width ||
                    gridPos.y < 0 || gridPos.y >= navGrid.height)
                {
                    Debug.LogWarning($"Object {obj.name} is outside grid bounds at {gridPos}");
                    continue;
                }

                // Obter par�metros do objeto
                int value = GetObjectValue(obj, objectType);
                int health = GetObjectHealth(obj, objectType);

                // Criar LevelObject e adicionar � lista
                LevelObject levelObject = new LevelObject(objectType, gridPos.x, gridPos.y, value, health);
                levelData.objects.Add(levelObject);
                objectCount++;
            }

            if (objectCount == 0)
            {
                Debug.LogWarning("No objects to save!");
                return "Warning: No objects in level";
            }

            // Serializar para JSON
            string json = JsonUtility.ToJson(levelData, true);

            // Criar diret�rio se n�o existir
            string directoryPath = Path.Combine(Application.persistentDataPath, "levels");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log($"Created directory: {directoryPath}");
            }

            // Caminho completo do arquivo
            string filePath = Path.Combine(directoryPath, fileName + ".json");

            // Salvar arquivo
            File.WriteAllText(filePath, json);

            string successMessage = $"Level saved: {fileName} ({objectCount} objects)";
            Debug.Log($"{successMessage} at {filePath}");
            return successMessage;
        }
        catch (System.Exception ex)
        {
            string errorMessage = $"Failed to save level: {ex.Message}";
            Debug.LogError(errorMessage);
            return errorMessage;
        }
    }

    /// <summary>
    /// Determina o tipo de um GameObject baseado em seus componentes.
    /// </summary>
    private static string DetermineObjectType(GameObject obj)
    {
        // Verificar componentes espec�ficos
        if (obj.GetComponent<Coin>() != null)
            return "Coin";

        if (obj.GetComponent<Chest>() != null)
            return "Chest";

        if (obj.GetComponent<Spike>() != null)
            return "Spike";

        if (obj.GetComponent<DestructibleBlockReinforced>() != null)
        {
            // Diferenciar entre bloco normal e refor�ado pelo health
            var block = obj.GetComponent<DestructibleBlockReinforced>();
            int health = block.GetHealth();
            return health == 1 ? "DestructibleBlock" : "DestructibleBlockReinforced";
        }

        return "";
    }

    /// <summary>
    /// Obt�m o valor de um objeto (usado por Coin e Chest).
    /// </summary>
    private static int GetObjectValue(GameObject obj, string objectType)
    {
        switch (objectType)
        {
            case "Coin":
                var coin = obj.GetComponent<Coin>();
                if (coin != null)
                {
                    return coin.GetCoinValue();
                }
                return 10; // Valor padr�o

            case "Chest":
                var chest = obj.GetComponent<Chest>();
                if (chest != null)
                {
                    return chest.GetChestValue();
                }
                return 100; // Valor padr�o

            default:
                return 0;
        }
    }

    /// <summary>
    /// Obt�m a vida de um objeto (usado por blocos destru�veis).
    /// </summary>
    private static int GetObjectHealth(GameObject obj, string objectType)
    {
        if (objectType == "DestructibleBlock" || objectType == "DestructibleBlockReinforced")
        {
            var block = obj.GetComponent<DestructibleBlockReinforced>();
            if (block != null)
            {
                return block.GetHealth();
            }
        }

        return 1; // Valor padr�o
    }
}
