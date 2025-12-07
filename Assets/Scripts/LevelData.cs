using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe serializável para armazenar dados de uma fase em formato JSON.
/// Contém informações sobre o grid e todos os objetos posicionados na fase.
/// </summary>
[System.Serializable]
public class LevelData
{
    // Dimensões e configurações do grid
    public int gridWidth;
    public int gridHeight;
    public float cellSize;
    public float gridOriginX;
    public float gridOriginY;

    // Lista de todos os objetos na fase
    public List<LevelObject> objects;

    public LevelData()
    {
        objects = new List<LevelObject>();
    }
}

/// <summary>
/// Representa um objeto individual na fase.
/// Armazena tipo, posição no grid e parâmetros específicos.
/// </summary>
[System.Serializable]
public class LevelObject
{
    // Tipo do objeto: "Coin", "Chest", "DestructibleBlock", "DestructibleBlockReinforced", "Spike"
    public string type;

    // Posição no grid (coordenadas do grid, não world position)
    public int gridX;
    public int gridY;

    // Parâmetros opcionais (nem todos os objetos usam todos os parâmetros)
    public int value;       // Usado por Coin e Chest
    public int health;      // Usado por blocos destrutíveis

    // Construtor padrão
    public LevelObject()
    {
        type = "";
        gridX = 0;
        gridY = 0;
        value = 0;
        health = 1;
    }

    // Construtor completo
    public LevelObject(string objectType, int x, int y, int objValue = 0, int objHealth = 1)
    {
        type = objectType;
        gridX = x;
        gridY = y;
        value = objValue;
        health = objHealth;
    }
}
