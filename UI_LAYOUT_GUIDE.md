# 🎨 Exemplo de Layout UI para Level Editor

## Canvas Hierarchy Completa

```
Canvas
├── EditorPanel (Panel)
│   ├── HeaderPanel
│   │   └── TitleText (Text): "LEVEL EDITOR"
│   │
│   ├── ObjectsPanel (Vertical Layout Group)
│   │   ├── SectionLabel (Text): "Objects:"
│   │   ├── CoinButton (Button): "💰 Coin (10)"
│   │   ├── ChestButton (Button): "🎁 Chest (100)"
│   │   ├── BlockButton (Button): "🧱 Block (1 HP)"
│   │   ├── BlockReinforcedButton (Button): "🧱🧱 Block (2 HP)"
│   │   └── SpikeButton (Button): "⚠️ Spike Trap"
│   │
│   ├── ActionsPanel (Vertical Layout Group)
│   │   ├── SectionLabel (Text): "Actions:"
│   │   ├── FileNameInput (InputField): "level_name"
│   │   ├── SaveButton (Button): "💾 Save Level"
│   │   ├── LoadButton (Button): "📂 Load Level"
│   │   ├── ClearButton (Button): "🗑️ Clear All"
│   │   │
│   │   ├── Spacer (Empty GameObject, Layout Element: 20px)
│   │   │
│   │   ├── EditModeButton (Button): "✏️ Edit Mode"
│   │   └── PlayButton (Button): "▶️ Play with AI"
│   │
│   └── StatusPanel
│       └── StatusText (Text): "[Status messages here]"
```

---

## Configurações Detalhadas

### **Canvas**

```
- Render Mode: Screen Space - Overlay
- Canvas Scaler:
  - UI Scale Mode: Scale With Screen Size
  - Reference Resolution: 1920 x 1080
```

### **EditorPanel (Panel)**

```
Rect Transform:
  - Anchors: Top-Left
  - Pivot: (0, 1)
  - Position: (10, -10, 0)
  - Width: 250
  - Height: 700

Image:
  - Color: (0.2, 0.2, 0.2, 0.9) [cinza escuro semi-transparente]

Padding:
  - Left: 10, Right: 10, Top: 10, Bottom: 10
```

### **HeaderPanel**

```
Layout Element:
  - Preferred Height: 50

Child: TitleText
  - Font: Bold
  - Font Size: 24
  - Color: White
  - Alignment: Center
  - Text: "LEVEL EDITOR"
```

### **ObjectsPanel (Vertical Layout Group)**

```
Vertical Layout Group:
  - Padding: 5
  - Spacing: 5
  - Child Alignment: Upper Center
  - Child Force Expand: Width ✓, Height ✗

Child: SectionLabel
  - Font Size: 16
  - Color: Yellow
  - Text: "═══ OBJECTS ═══"
  - Alignment: Center

Buttons (todos seguem o mesmo padrão):
  - Width: 230
  - Height: 40
  - Font Size: 16
  - Colors:
    - Normal: (0.3, 0.3, 0.3)
    - Highlighted: (0.5, 0.5, 0.5)
    - Pressed: (0.2, 0.5, 0.2)
    - Selected: (0.3, 0.6, 0.3)
```

### **ActionsPanel (Vertical Layout Group)**

```
Vertical Layout Group:
  - Same as ObjectsPanel

Spacer (Empty GameObject):
  - Layout Element:
    - Preferred Height: 20

FileNameInput (InputField):
  - Width: 230
  - Height: 30
  - Text: "my_level"
  - Placeholder: "Enter level name..."
  - Character Limit: 50
```

### **StatusPanel**

```
Layout Element:
  - Preferred Height: 120
  - Flexible Height: 1

StatusText:
  - Font Size: 14
  - Color: (0.8, 0.8, 0.8) [cinza claro]
  - Alignment: Top-Left
  - Vertical Overflow: Truncate
  - Best Fit: ✓
  - Min Size: 10, Max Size: 14
  - Text: "Status: Ready"
```

---

## Esquema Visual (ASCII Art)

```
┌──────────────────────────┐
│    LEVEL EDITOR          │ ← Header
├──────────────────────────┤
│  ═══ OBJECTS ═══         │ ← Section Label
│                          │
│  [💰 Coin (10)]          │
│  [🎁 Chest (100)]        │
│  [🧱 Block (1 HP)]       │
│  [🧱🧱 Block (2 HP)]      │
│  [⚠️ Spike Trap]         │
│                          │
│  ═══ ACTIONS ═══         │
│                          │
│  [level_name______]      │ ← InputField
│  [💾 Save Level]         │
│  [📂 Load Level]         │
│  [🗑️ Clear All]          │
│                          │
│         (space)          │
│                          │
│  [✏️ Edit Mode]          │
│  [▶️ Play with AI]       │
│                          │
│  ─────────────────────   │
│  Status: Ready           │ ← Status Text
│  Selected: Coin          │
│  Coin placed at (5, 3)   │
└──────────────────────────┘
```

---

## Cores Sugeridas

### Tema Escuro (Recomendado)

```csharp
// Painel Principal
Background: RGBA(50, 50, 50, 230)    // #323232E6

// Botões
Normal: RGBA(70, 70, 70, 255)        // #464646FF
Hover: RGBA(100, 100, 100, 255)      // #646464FF
Pressed: RGBA(50, 120, 50, 255)      // #327832FF
Selected: RGBA(70, 150, 70, 255)     // #469646FF

// Textos
Title: RGBA(255, 255, 255, 255)      // #FFFFFFFF
Section: RGBA(255, 220, 100, 255)    // #FFDC64FF
Status: RGBA(200, 200, 200, 255)     // #C8C8C8FF
```

### Tema Claro (Alternativo)

```csharp
// Painel Principal
Background: RGBA(220, 220, 220, 230) // #DCDCDCE6

// Botões
Normal: RGBA(180, 180, 180, 255)     // #B4B4B4FF
Hover: RGBA(150, 150, 150, 255)      // #969696FF
Pressed: RGBA(100, 200, 100, 255)    // #64C864FF
Selected: RGBA(120, 220, 120, 255)   // #78DC78FF

// Textos
Title: RGBA(40, 40, 40, 255)         // #282828FF
Section: RGBA(200, 100, 0, 255)      // #C86400FF
Status: RGBA(60, 60, 60, 255)        // #3C3C3CFF
```

---

## Ícones de Texto (Emojis) - Opcionais

Se não quiser usar emojis, use texto simples:

```
Coin Button: "Coin (10 pts)"
Chest Button: "Chest (100 pts)"
Block Button: "Block (1 HP)"
Block Reinforced: "Reinforced Block (2 HP)"
Spike Button: "Spike Trap"
Save Button: "Save Level"
Load Button: "Load Level"
Clear Button: "Clear All Objects"
Edit Button: "Enter Edit Mode"
Play Button: "Play with AI"
```

---

## Exemplo de Prefab UI (Opcional)

Você pode criar um prefab de botão reutilizável:

```
ButtonPrefab
├── Button (Button Component)
│   └── Text (Text Component)
│       └── Icon (Text Component) [opcional]
```

**Script para Botão Customizado:**

```csharp
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EditorButton : MonoBehaviour
{
    [SerializeField] private Text labelText;
    [SerializeField] private Text iconText;
    [SerializeField] private Color selectedColor = Color.green;

    private Image buttonImage;
    private Color originalColor;
    private bool isSelected = false;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;
    }

    public void SetLabel(string text)
    {
        if (labelText != null)
            labelText.text = text;
    }

    public void SetIcon(string icon)
    {
        if (iconText != null)
            iconText.text = icon;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        buttonImage.color = selected ? selectedColor : originalColor;
    }
}
```

---

## Animações UI (Opcional)

Para feedback visual mais elegante:

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float animationSpeed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * animationSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}
```

---

## Posicionamento Responsivo

Para diferentes resoluções, use Anchors:

```
Painel Esquerdo (EditorPanel):
  - Min: (0, 1)
  - Max: (0, 1)
  - Pivot: (0, 1)
  - Position: (10, -10)

Painel Direito (se adicionar preview):
  - Min: (1, 1)
  - Max: (1, 1)
  - Pivot: (1, 1)
  - Position: (-10, -10)

Painel Central (se adicionar info):
  - Min: (0.5, 0)
  - Max: (0.5, 0)
  - Pivot: (0.5, 0)
  - Position: (0, 10)
```

---

## Melhorias Futuras

1. **Dropdown de Fases Salvas**

   - Lista todos os arquivos .json na pasta levels/
   - Permite selecionar facilmente

2. **Preview da Fase**

   - Miniatura visual da fase
   - Mostra grid com objetos

3. **Painel de Propriedades**

   - Ajusta health, value dinamicamente
   - Permite customizar cor, sprite

4. **Barra de Ferramentas Superior**

   - Undo/Redo
   - Copy/Paste
   - Grid Size Selector

5. **Tabs/Categorias**
   - Tab "Objects"
   - Tab "Actions"
   - Tab "Settings"

---

Implemente este layout e ajuste conforme seu estilo visual! 🎨
