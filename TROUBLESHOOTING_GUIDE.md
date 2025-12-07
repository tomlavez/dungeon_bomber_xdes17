# 🔧 Troubleshooting e Exemplos Práticos

## 🐛 Problemas Comuns e Soluções

### **1. Compilação**

#### ❌ Erro: "Type or namespace 'Coin' could not be found"

**Causa:** Scripts Coin.cs, Chest.cs ou Spike.cs não existem ou não compilam.

**Solução:**

- Verifique se os scripts existem em Assets/Scripts/
- Abra cada script e verifique erros de compilação
- Certifique-se que os nomes das classes correspondem aos nomes dos arquivos

#### ❌ Erro: "NavGrid does not contain a definition for 'RefreshGrid'"

**Causa:** Método RefreshGrid() não foi adicionado ao NavGrid.cs

**Solução:**

```csharp
// Adicione este método ao NavGrid.cs
public void RefreshGrid()
{
    GenerateGrid();
    Debug.Log("[NavGrid] Grid refreshed");
}
```

---

### **2. Runtime - Placement**

#### ❌ Objetos não aparecem ao clicar

**Diagnóstico:**

1. Abra o Console (Ctrl+Shift+C)
2. Veja se há mensagens de erro
3. Verifique o StatusText na UI

**Possíveis causas:**

**A) Prefabs não conectados**

```
Error: Prefab not found for Coin
```

**Solução:** No LevelEditor Inspector, arraste todos os prefabs.

**B) Célula ocupada**

```
Cell occupied at (5, 3)
```

**Solução:** Use botão direito para remover objeto primeiro.

**C) Fora dos limites**

```
Invalid position: (15, 9) - Out of bounds
```

**Solução:** Clique dentro do grid (0-13 x, 0-7 y).

**D) Nenhum tipo selecionado**

```
No object type selected!
```

**Solução:** Clique em um botão de objeto primeiro (Coin, Chest, etc).

---

### **3. Runtime - Save/Load**

#### ❌ "Failed to save level: Access denied"

**Causa:** Permissões de escrita no disco.

**Solução:**

- Execute Unity como administrador (temporariamente)
- Verifique path: `Debug.Log(Application.persistentDataPath);`
- Verifique se pasta não está read-only

#### ❌ "File not found: my_level"

**Causa:** Arquivo não existe ou nome errado.

**Solução:**

1. Verifique path completo:

```csharp
string path = Application.persistentDataPath + "/levels/";
Debug.Log("Looking for files in: " + path);
```

2. Liste arquivos existentes:

```csharp
string[] files = System.IO.Directory.GetFiles(path, "*.json");
foreach (string file in files)
    Debug.Log("Found: " + System.IO.Path.GetFileName(file));
```

#### ❌ Objetos carregados em posições erradas

**Causa:** Coordenadas do grid mudaram.

**Solução:**

- Verifique `gridOrigin` no NavGrid
- Verifique `width` e `height` no NavGrid
- Certifique-se que correspondem ao JSON:

```json
"gridOriginX": 0.0,
"gridOriginY": 0.0,
"gridWidth": 14,
"gridHeight": 8
```

---

### **4. UI e Input**

#### ❌ Cliques não funcionam

**Causa:** EventSystem não existe ou múltiplos EventSystems.

**Solução:**

1. Hierarchy → Create → UI → Event System (se não existir)
2. Verifique que só há 1 EventSystem na cena

#### ❌ Clica no UI e coloca objeto

**Causa:** Raycast passa através do UI.

**Solução:**
Adicione verificação no LevelEditor.cs:

```csharp
using UnityEngine.EventSystems;

private void HandleLeftClick()
{
    // Ignorar cliques sobre UI
    if (EventSystem.current.IsPointerOverGameObject())
        return;

    // ... resto do código
}

private void HandleRightClick()
{
    if (EventSystem.current.IsPointerOverGameObject())
        return;

    // ... resto do código
}
```

#### ❌ Botões não respondem

**Causa:** Time.timeScale = 0 afeta UI.

**Solução:**
No Canvas, configure:

- Canvas → Render Mode: Screen Space - Overlay
- Canvas → Additional Shader Channels: Everything

---

### **5. Grid e Visualização**

#### ❌ Grid não aparece

**Causa:** GridVisualizer não está mostrando.

**Solução:**

**Scene View:** Grid sempre aparece via Gizmos

- Certifique-se que Gizmos estão habilitados (botão no canto superior direito)

**Game View:**

- No GridVisualizer Inspector, marque `Show In Game View`
- Chame `gridVisualizer.ShowGrid()`

#### ❌ Posições dos objetos não coincidem com grid visual

**Causa:** Anchor ou offset incorreto.

**Solução:**

- Verifique que objetos são posicionados no centro da célula
- No NavGrid.GridToWorld(), ajuste offset:

```csharp
public Vector2 GridToWorld(int x, int y)
{
    return new Vector2(
        gridOrigin.x + x * cellSize + cellSize * 0.5f, // Centro da célula
        gridOrigin.y - y * cellSize - cellSize * 0.5f
    );
}
```

---

### **6. IA e NavGrid**

#### ❌ IA não detecta objetos carregados

**Causa:** NavGrid não foi atualizado.

**Solução:**

- LevelLoader já chama `navGrid.RefreshGrid()` automaticamente
- Se necessário, chame manualmente:

```csharp
FindFirstObjectByType<NavGrid>().RefreshGrid();
```

#### ❌ IA considera blocos destrutíveis como obstáculos permanentes

**Causa:** Tag ou Layer incorretos.

**Solução:**
Verifique nos prefabs:

- DestructibleBlock: Tag "Destructible", Layer "Obstacle"
- DestructibleBlockReinforced: Tag "Destructible", Layer "Obstacle"

---

## 📚 Exemplos Práticos

### **Exemplo 1: Criar Fase Simples**

```
1. Play no Unity
2. Clique "Edit Mode"
3. Clique "Coin"
4. Clique em (3, 2) no grid → Coin aparece
5. Clique "Chest"
6. Clique em (10, 5) → Chest aparece
7. Clique "Block (1 HP)"
8. Clique em (5, 4) → Block aparece
9. Digite "tutorial_level" no InputField
10. Clique "Save Level"
11. Console mostra: "Level saved: tutorial_level (3 objects)"
```

**Resultado:** Arquivo criado em `persistentDataPath/levels/tutorial_level.json`

---

### **Exemplo 2: Carregar e Modificar Fase**

```
1. Play no Unity
2. Clique "Edit Mode"
3. Digite "tutorial_level" no InputField
4. Clique "Load Level"
5. Console mostra: "Level loaded: tutorial_level (3 objects)"
6. Objetos aparecem no grid
7. Clique direito no Coin em (3, 2) → Remove Coin
8. Clique "DestructibleBlockReinforced"
9. Clique em (7, 3) → Adiciona Block reforçado
10. Digite "tutorial_level_v2" no InputField
11. Clique "Save Level"
```

**Resultado:** Nova versão salva como `tutorial_level_v2.json`

---

### **Exemplo 3: Testar com IA**

```
1. Crie/carregue uma fase no Edit Mode
2. Certifique-se que há:
   - Pelo menos 1 Coin ou Chest (para IA coletar)
   - Alguns blocos destrutíveis
   - Espaço para IA navegar
3. Clique "Play with AI"
4. Observe IA navegando, coletando itens e destruindo blocos
5. Se IA não se move, verifique Console para erros
```

---

### **Exemplo 4: Criar Fase de Teste de Bomba**

```csharp
// Fase para testar destruição de blocos

1. Edit Mode
2. Coloque blocos em padrão:

   Grid Layout:
   [ ][ ][ ][ ][ ]
   [ ][B][B][B][ ]
   [ ][B][P][B][ ]   (P = Player spawn)
   [ ][B][B][B][ ]
   [ ][ ][ ][ ][ ]

3. Coloque 1 Chest no centro dos blocos
4. Salve como "bomb_test"
5. Play: IA deve usar bombas para destruir blocos e pegar Chest
```

---

### **Exemplo 5: Integrar com Script Customizado**

```csharp
using UnityEngine;

public class CustomLevelManager : MonoBehaviour
{
    [SerializeField] private LevelEditor levelEditor;

    // Lista de fases da campanha
    private string[] campaignLevels = {
        "level_01_tutorial",
        "level_02_maze",
        "level_03_boss"
    };

    private int currentLevelIndex = 0;

    void Start()
    {
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        if (currentLevelIndex < campaignLevels.Length)
        {
            string levelName = campaignLevels[currentLevelIndex];
            levelEditor.LoadLevel(levelName);
            Debug.Log($"Loaded: {levelName}");
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < campaignLevels.Length)
        {
            LoadCurrentLevel();
        }
        else
        {
            Debug.Log("Campaign complete!");
        }
    }

    public void RestartLevel()
    {
        LoadCurrentLevel();
    }

    public void LoadLevelByIndex(int index)
    {
        if (index >= 0 && index < campaignLevels.Length)
        {
            currentLevelIndex = index;
            LoadCurrentLevel();
        }
    }
}
```

---

## 🔍 Debug e Diagnóstico

### **Script de Diagnóstico**

Crie `LevelEditorDebug.cs`:

```csharp
using UnityEngine;
using System.IO;

public class LevelEditorDebug : MonoBehaviour
{
    [ContextMenu("Print Persistent Data Path")]
    void PrintPath()
    {
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    }

    [ContextMenu("List Saved Levels")]
    void ListLevels()
    {
        string path = Path.Combine(Application.persistentDataPath, "levels");

        if (!Directory.Exists(path))
        {
            Debug.LogWarning("Levels directory does not exist!");
            return;
        }

        string[] files = Directory.GetFiles(path, "*.json");
        Debug.Log($"Found {files.Length} level files:");

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            long fileSize = new FileInfo(file).Length;
            Debug.Log($"  - {fileName} ({fileSize} bytes)");
        }
    }

    [ContextMenu("Print Level JSON")]
    void PrintLastLevel()
    {
        string path = Path.Combine(Application.persistentDataPath, "levels");

        if (!Directory.Exists(path))
        {
            Debug.LogWarning("Levels directory does not exist!");
            return;
        }

        string[] files = Directory.GetFiles(path, "*.json");
        if (files.Length == 0)
        {
            Debug.LogWarning("No level files found!");
            return;
        }

        string lastFile = files[files.Length - 1];
        string json = File.ReadAllText(lastFile);
        Debug.Log($"Contents of {Path.GetFileName(lastFile)}:\n{json}");
    }

    [ContextMenu("Validate NavGrid")]
    void ValidateNavGrid()
    {
        NavGrid navGrid = FindFirstObjectByType<NavGrid>();

        if (navGrid == null)
        {
            Debug.LogError("NavGrid not found in scene!");
            return;
        }

        Debug.Log("NavGrid Configuration:");
        Debug.Log($"  Origin: {navGrid.gridOrigin}");
        Debug.Log($"  Size: {navGrid.width} x {navGrid.height}");
        Debug.Log($"  Cell Size: {navGrid.cellSize}");
        Debug.Log($"  Obstacle Mask: {navGrid.obstacleMask.value}");

        if (navGrid.grid != null)
        {
            int walkable = 0, blocked = 0, destructible = 0;

            for (int x = 0; x < navGrid.width; x++)
            {
                for (int y = 0; y < navGrid.height; y++)
                {
                    Node node = navGrid.grid[x, y];
                    if (node.walkable) walkable++;
                    else if (node.destructible) destructible++;
                    else blocked++;
                }
            }

            Debug.Log($"  Grid State: {walkable} walkable, {destructible} destructible, {blocked} blocked");
        }
    }
}
```

**Uso:**

1. Adicione ao GameObject LevelEditor
2. No Inspector, clique com botão direito no script
3. Use Context Menu para executar funções de debug

---

## 📊 Performance

### **Otimizações Recomendadas**

#### Para muitos objetos (>100):

```csharp
// Use Object Pooling ao invés de Instantiate/Destroy
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 50;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefab);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
```

---

## 🎓 Boas Práticas

1. **Sempre teste Save/Load** após criar uma fase
2. **Mantenha backups** de fases importantes (copie os JSON)
3. **Use nomes descritivos** para arquivos de fase
4. **Documente layouts** complexos com comentários
5. **Valide fases** antes de usar em produção (tem saída? é possível ganhar?)

---

**Problemas não listados aqui?**  
Verifique o Console do Unity para mensagens de erro detalhadas! 🔍
