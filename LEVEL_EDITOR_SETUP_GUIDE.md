# Guia de Configuração do Level Editor - Dungeon Bomber

## 📋 Resumo dos Scripts Criados

Foram criados 5 scripts principais e 1 modificação:

1. **LevelData.cs** - Estrutura de dados para serialização JSON
2. **LevelEditor.cs** - Gerenciador principal do modo de edição
3. **LevelSaver.cs** - Serializa e salva fases em JSON
4. **LevelLoader.cs** - Carrega fases de arquivos JSON
5. **GridVisualizer.cs** - Visualização do grid durante edição
6. **NavGrid.cs** - Adicionado método `RefreshGrid()`

---

## 🛠️ Configuração no Unity Editor

### **Passo 1: Criar GameObject LevelEditor**

1. Na Hierarchy, crie um GameObject vazio (botão direito → Create Empty)
2. Renomeie para **"LevelEditor"**
3. Adicione o script **LevelEditor.cs**
4. Adicione o script **GridVisualizer.cs** ao mesmo GameObject

### **Passo 2: Criar Container para Objetos**

1. Crie outro GameObject vazio na Hierarchy
2. Renomeie para **"LevelObjects"**
3. Este será o parent de todos os objetos criados no editor

### **Passo 3: Criar Interface UI (Canvas)**

Crie a seguinte estrutura de UI:

```
Canvas (Canvas UI)
├── EditorPanel (Panel)
│   ├── Title (Text) - "Level Editor"
│   ├── ButtonsPanel (Vertical Layout Group)
│   │   ├── CoinButton (Button) - "Coin"
│   │   ├── ChestButton (Button) - "Chest"
│   │   ├── BlockButton (Button) - "Block (1 HP)"
│   │   ├── BlockReinforcedButton (Button) - "Block (2 HP)"
│   │   └── SpikeButton (Button) - "Spike"
│   ├── ActionsPanel (Vertical Layout Group)
│   │   ├── ClearButton (Button) - "Clear All"
│   │   ├── SaveButton (Button) - "Save Level"
│   │   ├── LoadButton (Button) - "Load Level"
│   │   ├── EditModeButton (Button) - "Edit Mode"
│   │   └── PlayButton (Button) - "Play"
│   └── StatusText (Text) - Área para mensagens
```

#### Configurações Recomendadas:

**EditorPanel:**

- Anchor: Top-Left
- Width: 200, Height: 600
- Background: Semi-transparente (α = 0.8)

**Botões:**

- Width: 180, Height: 40
- Spacing: 10px entre botões

**StatusText:**

- Width: 180, Height: 100
- Font Size: 14
- Color: White
- Alignment: Top-Left

### **Passo 4: Conectar Referências no LevelEditor**

Selecione o GameObject **LevelEditor** no Inspector e configure:

#### **References:**

- **Nav Grid**: Arraste o GameObject que contém o NavGrid.cs da cena
- **Objects Container**: Arraste o GameObject "LevelObjects"

#### **Prefabs:**

Arraste os prefabs da pasta `Assets/Prefabs/`:

- **Coin Prefab**: Coin.prefab
- **Chest Prefab**: Chest.prefab
- **Destructible Block Prefab**: DestructibleBlock 1.prefab
- **Destructible Block Reinforced Prefab**: DestructibleBlockReinforced.prefab
- **Spike Prefab**: Trap.prefab

#### **UI References:**

- **Editor UI Panel**: Arraste o GameObject "EditorPanel"
- **Status Text**: Arraste o componente Text de "StatusText"

### **Passo 5: Conectar Referências no GridVisualizer**

No mesmo GameObject **LevelEditor**, configure o GridVisualizer:

- **Nav Grid**: Arraste o GameObject que contém NavGrid.cs
- **Grid Color**: Branco ou amarelo claro (RGB 255, 255, 255)
- **Line Width**: 0.02
- **Show In Game View**: ☑ (marcar se quiser ver no Game View)
- **Highlight Mouse Cell**: ☑ (marcar para highlight)
- **Highlight Color**: Amarelo (RGB 255, 255, 0)

### **Passo 6: Configurar Eventos dos Botões**

Para cada botão, vá no Inspector → Button Component → On Click():

#### **Botões de Seleção de Objetos:**

**CoinButton:**

- Adicione evento
- Arraste o GameObject "LevelEditor"
- Selecione: `LevelEditor.SelectObjectType`
- Parâmetro string: `Coin`

**ChestButton:**

- LevelEditor → `SelectObjectType`
- Parâmetro: `Chest`

**BlockButton:**

- LevelEditor → `SelectObjectType`
- Parâmetro: `DestructibleBlock`

**BlockReinforcedButton:**

- LevelEditor → `SelectObjectType`
- Parâmetro: `DestructibleBlockReinforced`

**SpikeButton:**

- LevelEditor → `SelectObjectType`
- Parâmetro: `Spike`

#### **Botões de Ação:**

**ClearButton:**

- LevelEditor → `LevelEditor.ClearAllObjects`

**SaveButton:**

- Este precisa de um InputField para o nome do arquivo
- Por enquanto, você pode criar um método auxiliar ou chamar diretamente:
- LevelEditor → `LevelEditor.SaveLevel`
- Parâmetro: `test_level` (ou criar InputField)

**LoadButton:**

- LevelEditor → `LevelEditor.LoadLevel`
- Parâmetro: `test_level` (ou criar InputField)

**EditModeButton:**

- LevelEditor → `LevelEditor.EnableEditMode`

**PlayButton:**

- LevelEditor → `LevelEditor.DisableEditMode`
- **IMPORTANTE**: Adicione também chamadas para:
  - Ativar AIController no Player
  - Desativar PlayerKeyboardController

### **Passo 7: Criar Script de Controle de Modo de Jogo (Opcional)**

Para facilitar a alternância entre Edit e Play com IA, crie um script `GameModeController.cs`:

```csharp
using UnityEngine;

public class GameModeController : MonoBehaviour
{
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private GameObject player;
    [SerializeField] private MonoBehaviour aiController;
    [SerializeField] private MonoBehaviour keyboardController;

    public void StartEditMode()
    {
        levelEditor.EnableEditMode();
        aiController.enabled = false;
        keyboardController.enabled = true;
    }

    public void StartPlayMode()
    {
        levelEditor.DisableEditMode();
        aiController.enabled = true;
        keyboardController.enabled = false;
    }
}
```

Conecte este script aos botões Edit e Play.

---

## 🎮 Como Usar o Level Editor

### **Modo de Edição:**

1. Clique no botão **"Edit Mode"**

   - O jogo pausará (Time.timeScale = 0)
   - O painel de ferramentas aparecerá

2. **Selecionar objeto:**

   - Clique em um dos botões (Coin, Chest, Block, etc.)
   - O status mostrará: "Selected: [tipo]"

3. **Colocar objeto:**

   - Clique com **botão esquerdo** do mouse em uma célula vazia do grid
   - Objeto será instanciado na posição
   - Status mostrará: "[tipo] placed at (x, y)"

4. **Remover objeto:**

   - Clique com **botão direito** do mouse sobre um objeto
   - Objeto será destruído
   - Status mostrará: "[tipo] removed from (x, y)"

5. **Limpar fase:**
   - Clique em **"Clear All"**
   - Todos os objetos serão removidos

### **Salvar e Carregar:**

**Salvar:**

1. Construa sua fase no editor
2. Clique em **"Save Level"**
3. Fase será salva em: `Application.persistentDataPath/levels/[nome].json`
4. No Windows: `C:\Users\[user]\AppData\LocalLow\[company]\[project]\levels\`

**Carregar:**

1. Clique em **"Load Level"**
2. Fase será carregada do arquivo JSON
3. Objetos existentes serão removidos
4. Novos objetos serão instanciados
5. NavGrid será atualizado automaticamente

### **Testar com IA:**

1. Termine de editar a fase
2. Clique em **"Play"**
   - Modo de edição será desativado
   - Jogo será despausado
   - IA começará a jogar

---

## 📁 Estrutura do Arquivo JSON

Exemplo de fase salva:

```json
{
  "gridWidth": 14,
  "gridHeight": 8,
  "cellSize": 1.0,
  "gridOriginX": 0.0,
  "gridOriginY": 0.0,
  "objects": [
    {
      "type": "Coin",
      "gridX": 3,
      "gridY": 2,
      "value": 10,
      "health": 1
    },
    {
      "type": "Chest",
      "gridX": 10,
      "gridY": 5,
      "value": 100,
      "health": 1
    },
    {
      "type": "DestructibleBlock",
      "gridX": 5,
      "gridY": 4,
      "value": 0,
      "health": 1
    },
    {
      "type": "DestructibleBlockReinforced",
      "gridX": 7,
      "gridY": 3,
      "value": 0,
      "health": 2
    },
    {
      "type": "Spike",
      "gridX": 2,
      "gridY": 6,
      "value": 0,
      "health": 1
    }
  ]
}
```

---

## 🐛 Solução de Problemas

### **"Cell occupied!" ao clicar**

- Já existe um objeto nessa célula
- Use botão direito para remover primeiro

### **"Invalid position: Out of bounds"**

- Clicou fora dos limites do grid (14×8)
- Clique dentro da área do grid

### **"Prefab not found for [tipo]"**

- Verifique se todos os prefabs estão conectados no Inspector
- Verifique os nomes dos prefabs na pasta Assets/Prefabs/

### **Objetos não aparecem após Load**

- Verifique o Console para erros
- Confirme que o arquivo JSON existe em persistentDataPath/levels/
- Verifique se NavGrid está conectado

### **Grid não aparece visualmente**

- No GridVisualizer, marque "Show In Game View"
- Ou verifique Scene View (Gizmos sempre aparecem lá)

### **IA não funciona após carregar fase**

- NavGrid.RefreshGrid() é chamado automaticamente
- Verifique se objetos têm Layer "Obstacle" correto
- Confirme que blocos têm tag "Destructible"

---

## ⚙️ Configurações Avançadas

### **Adicionar InputField para nome de arquivo:**

1. Adicione um InputField no EditorPanel
2. Crie um script intermediário:

```csharp
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private InputField fileNameInput;

    public void SaveLevelWithInput()
    {
        string fileName = fileNameInput.text;
        if (string.IsNullOrEmpty(fileName))
            fileName = "unnamed_level";

        levelEditor.SaveLevel(fileName);
    }

    public void LoadLevelWithInput()
    {
        string fileName = fileNameInput.text;
        if (string.IsNullOrEmpty(fileName))
            return;

        levelEditor.LoadLevel(fileName);
    }
}
```

### **Atalhos de Teclado:**

Adicione no Update() do LevelEditor:

```csharp
if (Input.GetKeyDown(KeyCode.E))
    EnableEditMode();

if (Input.GetKeyDown(KeyCode.P))
    DisableEditMode();

if (Input.GetKeyDown(KeyCode.Alpha1))
    SelectObjectType("Coin");

if (Input.GetKeyDown(KeyCode.Alpha2))
    SelectObjectType("Chest");
// etc...
```

---

## ✅ Checklist de Configuração

- [ ] Scripts compilam sem erros
- [ ] GameObject LevelEditor criado com scripts
- [ ] GameObject LevelObjects criado
- [ ] Canvas UI criado com todos os botões
- [ ] Referências conectadas no LevelEditor Inspector
- [ ] Referências conectadas no GridVisualizer Inspector
- [ ] Eventos dos botões configurados
- [ ] Teste: Entrar em Edit Mode
- [ ] Teste: Colocar um Coin
- [ ] Teste: Remover um objeto
- [ ] Teste: Salvar fase
- [ ] Teste: Carregar fase
- [ ] Teste: Jogar com IA após carregar

---

## 📝 Notas Importantes

1. **Persistência de Dados:** Arquivos são salvos em `Application.persistentDataPath`. Para compartilhar fases, copie os arquivos JSON desta pasta.

2. **Limitações Atuais:**

   - Nome do arquivo é fixo no código (use InputField para tornar dinâmico)
   - Não há lista de arquivos salvos (pode adicionar um Dropdown)
   - Não há undo/redo

3. **Melhorias Futuras Sugeridas:**

   - Sistema de lista de fases salvas
   - Preview de fases antes de carregar
   - Copiar/colar áreas
   - Ferramenta de preenchimento
   - Validação de fases (verificar se tem saída, etc.)

4. **Compatibilidade:**
   - Scripts funcionam com Legacy Input System
   - Para New Input System, substitua `Input.GetMouseButtonDown()` por callbacks

---

## 🎯 Resultado Esperado

Após seguir este guia, você terá:

- ✅ Um editor visual funcional no Game View
- ✅ Capacidade de criar fases customizadas
- ✅ Sistema de save/load em JSON
- ✅ Alternância entre edição e teste com IA
- ✅ Visualização do grid durante edição
- ✅ Feedback visual e textual de todas as ações

**Local dos arquivos salvos no Windows:**

```
C:\Users\[SEU_USUARIO]\AppData\LocalLow\[CompanyName]\DungeonBomber\levels\
```

Boa sorte com o desenvolvimento! 🚀
