# 🎮 Level Editor - Dungeon Bomber

Sistema completo de editor de fases para Unity com save/load em JSON.

## 📦 Scripts Criados

### Scripts Principais (obrigatórios)

1. **LevelData.cs** - Estrutura de dados JSON
2. **LevelEditor.cs** - Gerenciador do modo de edição
3. **LevelSaver.cs** - Salva fases em JSON
4. **LevelLoader.cs** - Carrega fases de JSON
5. **GridVisualizer.cs** - Visualização do grid

### Scripts Auxiliares (opcionais)

6. **LevelEditorUI.cs** - Interface facilitada com InputField
7. **GameModeController.cs** - Alterna entre Edit/Play com IA

### Modificações

- **NavGrid.cs** - Adicionado método `RefreshGrid()`

---

## 🚀 Quick Start

### 1. Setup Básico

**Criar na Hierarchy:**

- GameObject vazio `LevelEditor` (com scripts LevelEditor.cs e GridVisualizer.cs)
- GameObject vazio `LevelObjects` (container para objetos da fase)

**Criar UI Canvas com:**

- Painel de botões (Coin, Chest, Block, BlockReinforced, Spike)
- Botões de ação (Save, Load, Clear, Edit, Play)
- Text para status

### 2. Conectar Referências

**No LevelEditor Inspector:**

```
References:
  ✓ Nav Grid → (seu NavGrid da cena)
  ✓ Objects Container → LevelObjects

Prefabs:
  ✓ Coin Prefab → Coin.prefab
  ✓ Chest Prefab → Chest.prefab
  ✓ Destructible Block Prefab → DestructibleBlock 1.prefab
  ✓ Destructible Block Reinforced Prefab → DestructibleBlockReinforced.prefab
  ✓ Spike Prefab → Trap.prefab

UI References:
  ✓ Editor UI Panel → (seu painel UI)
  ✓ Status Text → (seu Text component)
```

**No GridVisualizer Inspector:**

```
  ✓ Nav Grid → (seu NavGrid da cena)
  ✓ Grid Color → Branco (255, 255, 255)
  ✓ Show In Game View → ☑ (opcional)
```

### 3. Configurar Botões

**Botões de Objetos → `LevelEditor.SelectObjectType(string)`:**

- Coin → `"Coin"`
- Chest → `"Chest"`
- Block → `"DestructibleBlock"`
- BlockReinforced → `"DestructibleBlockReinforced"`
- Spike → `"Spike"`

**Botões de Ação:**

- Clear → `LevelEditor.ClearAllObjects()`
- Save → `LevelEditor.SaveLevel(string)`
- Load → `LevelEditor.LoadLevel(string)`
- Edit → `LevelEditor.EnableEditMode()`
- Play → `LevelEditor.DisableEditMode()`

---

## 🎯 Como Usar

### Modo Edição

1. Clique **Edit Mode**
2. Selecione um objeto (Coin, Chest, etc)
3. **Clique esquerdo** no grid → coloca objeto
4. **Clique direito** no objeto → remove objeto
5. Clique **Clear All** → limpa tudo

### Salvar/Carregar

- **Save:** Clique Save (fase salva em `persistentDataPath/levels/[nome].json`)
- **Load:** Clique Load (carrega fase do arquivo JSON)

### Testar

1. Clique **Play** → desativa edição, inicia jogo
2. IA joga automaticamente (configure no GameModeController)

---

## 📁 Formato JSON

```json
{
  "gridWidth": 14,
  "gridHeight": 8,
  "cellSize": 1.0,
  "gridOriginX": 0.0,
  "gridOriginY": 0.0,
  "objects": [
    { "type": "Coin", "gridX": 3, "gridY": 2, "value": 10, "health": 1 },
    { "type": "Chest", "gridX": 10, "gridY": 5, "value": 100, "health": 1 },
    {
      "type": "DestructibleBlock",
      "gridX": 5,
      "gridY": 4,
      "value": 0,
      "health": 1
    }
  ]
}
```

**Arquivos salvos em:**

- Windows: `C:\Users\[user]\AppData\LocalLow\[company]\[project]\levels\`
- Mac: `~/Library/Application Support/[company]/[project]/levels/`

---

## 🛠️ Scripts Opcionais

### LevelEditorUI.cs

Interface melhorada com InputField para nome de arquivo.

**Adicione ao Canvas:**

- InputField para nome do arquivo
- Configure LevelEditorUI com referências

**Use nos botões:**

- `LevelEditorUI.SaveLevelWithInput()`
- `LevelEditorUI.LoadLevelWithInput()`

### GameModeController.cs

Controla alternância Edit/Play e ativa/desativa IA.

**Configure no Inspector:**

- Level Editor
- Player GameObject
- AI Controller (MonoBehaviour)
- Player Keyboard Controller (MonoBehaviour)

**Atalhos:**

- `E` → Edit Mode
- `P` → Play with AI
- `K` → Play with Keyboard

---

## ✅ Checklist

- [ ] Scripts compilam sem erros
- [ ] LevelEditor GameObject criado
- [ ] LevelObjects container criado
- [ ] Canvas UI criado
- [ ] Botões configurados com eventos
- [ ] Referências conectadas no Inspector
- [ ] Teste: Colocar e remover objetos
- [ ] Teste: Salvar fase
- [ ] Teste: Carregar fase
- [ ] Teste: Jogar com IA

---

## 📖 Documentação Completa

Consulte **LEVEL_EDITOR_SETUP_GUIDE.md** para:

- Instruções detalhadas passo a passo
- Solução de problemas
- Configurações avançadas
- Melhorias futuras sugeridas

---

## 🎨 Recursos

- ✅ Coloca/remove objetos no grid
- ✅ Visualização do grid
- ✅ Highlight da célula do mouse
- ✅ Save/Load em JSON
- ✅ Validação de posições
- ✅ Feedback visual e textual
- ✅ Integração com NavGrid
- ✅ Suporte para 5 tipos de objetos
- ✅ Configuração de parâmetros (health, value)

---

**Desenvolvido para Unity 6.2**  
**Compatível com C# 9.0+**  
**Input System: Legacy (pode ser adaptado para New Input System)**

Bom desenvolvimento! 🚀
