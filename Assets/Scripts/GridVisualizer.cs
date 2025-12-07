using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script para visualizar o grid durante a edição.
/// Desenha linhas do grid na Scene View e opcionalmente no Game View.
/// </summary>
public class GridVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavGrid navGrid;

    [Header("Grid Visualization Settings")]
    [SerializeField] private Color gridColor = Color.white;
    [SerializeField] private float lineWidth = 0.02f;
    [SerializeField] private bool showInGameView = false; // Se true, cria LineRenderers

    [Header("Highlight Settings")]
    [SerializeField] private bool highlightMouseCell = true;
    [SerializeField] private Color highlightColor = Color.yellow;

    // Para renderiza��o no Game View
    private GameObject gridLinesContainer;
    private bool isGridVisible = false;

    void Start()
    {
        // Grid come�a oculto
        HideGrid();
    }

    void Update()
    {
        // Atualizar highlight se necess�rio (apenas no Scene View via Gizmos)
    }

    /// <summary>
    /// Mostra o grid criando linhas visuais.
    /// </summary>
    public void ShowGrid()
    {
        if (navGrid == null)
        {
            Debug.LogError("NavGrid not assigned!");
            return;
        }

        if (showInGameView && !isGridVisible)
        {
            CreateGridLines();
        }

        isGridVisible = true;
        Debug.Log("Grid visualization enabled");
    }

    /// <summary>
    /// Esconde o grid removendo linhas visuais.
    /// </summary>
    public void HideGrid()
    {
        if (gridLinesContainer != null)
        {
            Destroy(gridLinesContainer);
            gridLinesContainer = null;
        }

        isGridVisible = false;
        Debug.Log("Grid visualization disabled");
    }

    /// <summary>
    /// Alterna visibilidade do grid.
    /// </summary>
    public void ToggleGrid()
    {
        if (isGridVisible)
            HideGrid();
        else
            ShowGrid();
    }

    /// <summary>
    /// Cria linhas do grid usando LineRenderers (vis�vel no Game View).
    /// </summary>
    private void CreateGridLines()
    {
        if (navGrid == null) return;

        // Criar container para organizar as linhas
        gridLinesContainer = new GameObject("GridLines");
        gridLinesContainer.transform.SetParent(transform);

        // Material padr�o para LineRenderer
        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lineMaterial.color = gridColor;

        // Linhas verticais
        for (int x = 0; x <= navGrid.width; x++)
        {
            GameObject lineObj = new GameObject($"VerticalLine_{x}");
            lineObj.transform.SetParent(gridLinesContainer.transform);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startColor = gridColor;
            lr.endColor = gridColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;

            // Ajustar para borda da célula (subtrair metade do cellSize)
            Vector2 start = navGrid.GridToWorld(x, 0) + new Vector2(-navGrid.cellSize * 0.5f, navGrid.cellSize * 0.5f);
            Vector2 end = navGrid.GridToWorld(x, navGrid.height) + new Vector2(-navGrid.cellSize * 0.5f, navGrid.cellSize * 0.5f);

            lr.SetPosition(0, new Vector3(start.x, start.y, 0));
            lr.SetPosition(1, new Vector3(end.x, end.y, 0));

            // Configurar sorting layer para ficar atr�s dos objetos
            lr.sortingLayerName = "Default";
            lr.sortingOrder = -10;
        }

        // Linhas horizontais
        for (int y = 0; y <= navGrid.height; y++)
        {
            GameObject lineObj = new GameObject($"HorizontalLine_{y}");
            lineObj.transform.SetParent(gridLinesContainer.transform);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startColor = gridColor;
            lr.endColor = gridColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;

            // Ajustar para borda da célula (subtrair metade do cellSize)
            Vector2 start = navGrid.GridToWorld(0, y) + new Vector2(-navGrid.cellSize * 0.5f, navGrid.cellSize * 0.5f);
            Vector2 end = navGrid.GridToWorld(navGrid.width, y) + new Vector2(-navGrid.cellSize * 0.5f, navGrid.cellSize * 0.5f);

            lr.SetPosition(0, new Vector3(start.x, start.y, 0));
            lr.SetPosition(1, new Vector3(end.x, end.y, 0));

            // Configurar sorting layer para ficar atr�s dos objetos
            lr.sortingLayerName = "Default";
            lr.sortingOrder = -10;
        }

        Debug.Log($"Created {navGrid.width + navGrid.height + 2} grid lines");
    }

    /// <summary>
    /// Desenha o grid na Scene View usando Gizmos (sempre vis�vel no editor).
    /// </summary>
    void OnDrawGizmos()
    {
        if (navGrid == null) return;

        Gizmos.color = gridColor;

        // Desenhar linhas verticais (ajustadas para bordas)
        for (int x = 0; x <= navGrid.width; x++)
        {
            Vector2 start = navGrid.GridToWorld(x, 0) + new Vector2(-navGrid.cellSize * 0.5f, navGrid.cellSize * 0.5f);
            Vector2 end = navGrid.GridToWorld(x, navGrid.height) + new Vector2(-navGrid.cellSize * 0.5f, navGrid.cellSize * 0.5f);
            Gizmos.DrawLine(start, end);
        }

        // Desenhar linhas horizontais (ajustadas para bordas)
        for (int y = 0; y <= navGrid.height; y++)
        {
            Vector2 start = navGrid.GridToWorld(0, y) + new Vector2(-navGrid.cellSize * 0.5f, navGrid.cellSize * 0.5f);
            Vector2 end = navGrid.GridToWorld(navGrid.width, y) + new Vector2(-navGrid.cellSize * 0.5f, navGrid.cellSize * 0.5f);
            Gizmos.DrawLine(start, end);
        }

        // Highlight da c�lula do mouse (se modo edi��o ativo)
        if (highlightMouseCell && Application.isPlaying)
        {
            DrawMouseCellHighlight();
        }
    }

    /// <summary>
    /// Desenha um highlight na c�lula onde o mouse est� posicionado.
    /// </summary>
    private void DrawMouseCellHighlight()
    {
        // Obter posição do mouse no mundo
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Converter para coordenadas do grid
        Node node = navGrid.WorldToNode(mouseWorldPos);
        Vector2Int gridPos = new Vector2Int(node.x, node.y);

        // Validar limites
        if (gridPos.x >= 0 && gridPos.x < navGrid.width &&
            gridPos.y >= 0 && gridPos.y < navGrid.height)
        {
            // Desenhar retângulo destacando a célula (ajustado para bordas)
            Gizmos.color = highlightColor;
            float offset = navGrid.cellSize * 0.5f;

            Vector2 bottomLeft = navGrid.GridToWorld(gridPos.x, gridPos.y) + new Vector2(-offset, offset);
            Vector2 bottomRight = navGrid.GridToWorld(gridPos.x + 1, gridPos.y) + new Vector2(-offset, offset);
            Vector2 topLeft = navGrid.GridToWorld(gridPos.x, gridPos.y + 1) + new Vector2(-offset, offset);
            Vector2 topRight = navGrid.GridToWorld(gridPos.x + 1, gridPos.y + 1) + new Vector2(-offset, offset);

            // Desenhar as 4 linhas do ret�ngulo
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }

    /// <summary>
    /// Desenha linhas do grid apenas quando o GameObject est� selecionado (Scene View).
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (navGrid == null) return;

        // Desenhar borda do grid em cor diferente (ajustada para bordas)
        Gizmos.color = Color.green;
        float offset = navGrid.cellSize * 0.5f;

        Vector2 bottomLeft = navGrid.GridToWorld(0, 0) + new Vector2(-offset, offset);
        Vector2 bottomRight = navGrid.GridToWorld(navGrid.width, 0) + new Vector2(-offset, offset);
        Vector2 topLeft = navGrid.GridToWorld(0, navGrid.height) + new Vector2(-offset, offset);
        Vector2 topRight = navGrid.GridToWorld(navGrid.width, navGrid.height) + new Vector2(-offset, offset);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
