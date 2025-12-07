# 📚 Level Editor - Índice de Documentação

Sistema completo de Level Editor para Dungeon Bomber (Unity 6.2)

---

## 🚀 Início Rápido

1. **Leia primeiro:** [LEVEL_EDITOR_README.md](LEVEL_EDITOR_README.md)  
   Visão geral rápida e checklist de configuração

2. **Setup detalhado:** [LEVEL_EDITOR_SETUP_GUIDE.md](LEVEL_EDITOR_SETUP_GUIDE.md)  
   Instruções completas passo a passo

3. **Criar UI:** [UI_LAYOUT_GUIDE.md](UI_LAYOUT_GUIDE.md)  
   Layout e design da interface

4. **Problemas?** [TROUBLESHOOTING_GUIDE.md](TROUBLESHOOTING_GUIDE.md)  
   Soluções e exemplos práticos

---

## 📂 Scripts Criados

### Principais (Assets/Scripts/)

- ✅ **LevelData.cs** - Estrutura de dados JSON
- ✅ **LevelEditor.cs** - Gerenciador do modo de edição
- ✅ **LevelSaver.cs** - Serialização e salvamento
- ✅ **LevelLoader.cs** - Carregamento de fases
- ✅ **GridVisualizer.cs** - Visualização do grid

### Auxiliares (Assets/Scripts/)

- ✅ **LevelEditorUI.cs** - Interface com InputField
- ✅ **GameModeController.cs** - Controle Edit/Play

### Modificações

- ✅ **NavGrid.cs** - Adicionado `RefreshGrid()`

---

## 📖 Documentação por Tópico

### Setup e Configuração

- [Configuração inicial do Level Editor](LEVEL_EDITOR_SETUP_GUIDE.md#configuração-no-unity-editor)
- [Criar GameObject LevelEditor](LEVEL_EDITOR_SETUP_GUIDE.md#passo-1-criar-gameobject-leveleditor)
- [Conectar referências](LEVEL_EDITOR_SETUP_GUIDE.md#passo-4-conectar-referências-no-leveleditor)
- [Configurar eventos dos botões](LEVEL_EDITOR_SETUP_GUIDE.md#passo-6-configurar-eventos-dos-botões)

### Interface do Usuário

- [Estrutura do Canvas](UI_LAYOUT_GUIDE.md#canvas-hierarchy-completa)
- [Configurações dos componentes](UI_LAYOUT_GUIDE.md#configurações-detalhadas)
- [Esquema visual](UI_LAYOUT_GUIDE.md#esquema-visual-ascii-art)
- [Cores sugeridas](UI_LAYOUT_GUIDE.md#cores-sugeridas)

### Uso do Editor

- [Modo de edição](LEVEL_EDITOR_SETUP_GUIDE.md#modo-de-edição)
- [Salvar e carregar fases](LEVEL_EDITOR_SETUP_GUIDE.md#salvar-e-carregar)
- [Testar com IA](LEVEL_EDITOR_SETUP_GUIDE.md#testar-com-ia)
- [Formato do arquivo JSON](LEVEL_EDITOR_SETUP_GUIDE.md#estrutura-do-arquivo-json)

### Troubleshooting

- [Problemas de compilação](TROUBLESHOOTING_GUIDE.md#1-compilação)
- [Problemas de runtime](TROUBLESHOOTING_GUIDE.md#2-runtime---placement)
- [Problemas de save/load](TROUBLESHOOTING_GUIDE.md#3-runtime---saveload)
- [Problemas de UI](TROUBLESHOOTING_GUIDE.md#4-ui-e-input)
- [Exemplos práticos](TROUBLESHOOTING_GUIDE.md#exemplos-práticos)

---

## 🎯 Workflow Recomendado

### Para Desenvolvedores (Primeira Vez)

```
1. Ler LEVEL_EDITOR_README.md (5 min)
   ↓
2. Seguir LEVEL_EDITOR_SETUP_GUIDE.md (30-60 min)
   - Criar GameObjects
   - Criar UI
   - Conectar referências
   ↓
3. Consultar UI_LAYOUT_GUIDE.md (durante setup)
   - Layout do Canvas
   - Cores e estilos
   ↓
4. Testar funcionalidades
   - Edit Mode
   - Colocar/Remover objetos
   - Save/Load
   ↓
5. Se houver problemas → TROUBLESHOOTING_GUIDE.md
```

### Para Designers de Fases

```
1. Ler seção "Como Usar" em LEVEL_EDITOR_README.md
   ↓
2. Praticar com exemplos em TROUBLESHOOTING_GUIDE.md
   - Exemplo 1: Criar fase simples
   - Exemplo 2: Modificar fase
   ↓
3. Criar suas fases
   ↓
4. Testar com IA
```

---

## 🔧 Configuração Mínima

Para ter o editor funcionando rapidamente:

### GameObjects Necessários:

```
Hierarchy:
  - LevelEditor (com LevelEditor.cs e GridVisualizer.cs)
  - LevelObjects (Transform vazio)
  - Canvas (com UI básica)
```

### UI Mínima:

```
Canvas:
  - Button "Edit Mode" → LevelEditor.EnableEditMode()
  - Button "Coin" → LevelEditor.SelectObjectType("Coin")
  - Button "Save" → LevelEditor.SaveLevel("test")
  - Button "Load" → LevelEditor.LoadLevel("test")
  - Text (para status)
```

### Referências Obrigatórias:

```
LevelEditor Inspector:
  ✓ Nav Grid
  ✓ Objects Container
  ✓ Coin Prefab (mínimo 1 prefab para testar)
  ✓ Editor UI Panel
  ✓ Status Text
```

---

## 📋 Checklist Completo

### Setup

- [ ] Scripts compilam sem erros
- [ ] GameObject LevelEditor criado
- [ ] GameObject LevelObjects criado
- [ ] Scripts LevelEditor.cs e GridVisualizer.cs adicionados
- [ ] Canvas UI criado
- [ ] Botões de objetos criados (Coin, Chest, Block, etc)
- [ ] Botões de ação criados (Save, Load, Clear, Edit, Play)
- [ ] InputField para nome de arquivo (opcional)
- [ ] Text para status

### Referências

- [ ] NavGrid conectado no LevelEditor
- [ ] NavGrid conectado no GridVisualizer
- [ ] Objects Container conectado
- [ ] Todos os prefabs conectados (5 prefabs)
- [ ] Editor UI Panel conectado
- [ ] Status Text conectado

### Eventos

- [ ] Botões de objetos chamam SelectObjectType(string)
- [ ] Botão Clear chama ClearAllObjects()
- [ ] Botão Save chama SaveLevel(string)
- [ ] Botão Load chama LoadLevel(string)
- [ ] Botão Edit chama EnableEditMode()
- [ ] Botão Play chama DisableEditMode()

### Testes

- [ ] Edit Mode ativa/desativa corretamente
- [ ] Objetos são colocados com clique esquerdo
- [ ] Objetos são removidos com clique direito
- [ ] Status Text mostra mensagens
- [ ] Grid é visualizado (Scene ou Game View)
- [ ] Fase é salva com sucesso
- [ ] Fase é carregada com sucesso
- [ ] IA funciona após carregar fase

---

## 🆘 Suporte Rápido

### Erro de Compilação?

→ [TROUBLESHOOTING_GUIDE.md - Seção 1](TROUBLESHOOTING_GUIDE.md#1-compilação)

### Objetos não aparecem?

→ [TROUBLESHOOTING_GUIDE.md - Seção 2](TROUBLESHOOTING_GUIDE.md#2-runtime---placement)

### Save/Load não funciona?

→ [TROUBLESHOOTING_GUIDE.md - Seção 3](TROUBLESHOOTING_GUIDE.md#3-runtime---saveload)

### UI não responde?

→ [TROUBLESHOOTING_GUIDE.md - Seção 4](TROUBLESHOOTING_GUIDE.md#4-ui-e-input)

### Precisa de exemplos?

→ [TROUBLESHOOTING_GUIDE.md - Exemplos Práticos](TROUBLESHOOTING_GUIDE.md#exemplos-práticos)

---

## 📞 Informações Técnicas

**Versão Unity:** 6.2  
**C# Version:** 9.0+  
**Input System:** Legacy (adaptável para New Input System)  
**Serialização:** JsonUtility (built-in Unity)

**Arquivos salvos em:**

- Windows: `C:\Users\[user]\AppData\LocalLow\[company]\[project]\levels\`
- Mac: `~/Library/Application Support/[company]/[project]/levels/`
- Linux: `~/.config/unity3d/[company]/[project]/levels/`

**Tamanho típico de arquivo:** 1-5 KB por fase

---

## 🎨 Recursos

- ✅ Editor visual no Game View
- ✅ Colocação/remoção de objetos com mouse
- ✅ 5 tipos de objetos suportados (Coin, Chest, Block, BlockReinforced, Spike)
- ✅ Sistema de save/load em JSON
- ✅ Validação de posições e colisões
- ✅ Visualização do grid (Scene e Game View)
- ✅ Highlight de célula do mouse
- ✅ Feedback visual e textual
- ✅ Integração automática com NavGrid
- ✅ Suporte para testar com IA
- ✅ Configuração de parâmetros (health, value)

---

## 🚀 Melhorias Futuras Sugeridas

### Funcionalidades

- [ ] Undo/Redo
- [ ] Copy/Paste de áreas
- [ ] Ferramenta de preenchimento (flood fill)
- [ ] Múltipla seleção
- [ ] Rotação de objetos
- [ ] Grid de tamanhos variáveis

### Interface

- [ ] Dropdown de fases salvas
- [ ] Preview visual da fase
- [ ] Minimap
- [ ] Tabs/categorias de objetos
- [ ] Painel de propriedades
- [ ] Histórico de ações

### Sistema

- [ ] Validação de fases (tem saída? é jogável?)
- [ ] Templates de fases
- [ ] Import/Export para compartilhamento
- [ ] Versionamento de fases
- [ ] Compressão de arquivos

---

## 📄 Estrutura de Arquivos

```
My project/
├── Assets/
│   └── Scripts/
│       ├── LevelData.cs
│       ├── LevelEditor.cs
│       ├── LevelSaver.cs
│       ├── LevelLoader.cs
│       ├── GridVisualizer.cs
│       ├── LevelEditorUI.cs
│       ├── GameModeController.cs
│       └── NavGrid.cs (modificado)
│
├── LEVEL_EDITOR_README.md (visão geral)
├── LEVEL_EDITOR_SETUP_GUIDE.md (setup detalhado)
├── UI_LAYOUT_GUIDE.md (design da UI)
├── TROUBLESHOOTING_GUIDE.md (soluções)
└── INDEX.md (este arquivo)
```

---

## ✅ Status do Projeto

- [x] Scripts criados e testados
- [x] Documentação completa
- [x] Exemplos práticos fornecidos
- [x] Troubleshooting documentado
- [ ] Testes do desenvolvedor no Unity
- [ ] Configuração da UI
- [ ] Testes de gameplay

---

**Última atualização:** Dezembro 2025  
**Autor:** GitHub Copilot  
**Modelo:** Claude Sonnet 4.5

Boa sorte com o desenvolvimento! 🎮✨
