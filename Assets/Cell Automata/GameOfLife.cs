using System;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [Header("Game size settings")]
    public int columns = 10;
    public int rows = 10;

    [Header("States settings")]
    public int lonelinessValue = 2;
    [Range(0.0f, 1.0f)]
    public float dieChanceLoneliness = 0.6f;
    public int overpopulationValue = 3;
    [Range(0.0f, 1.0f)]
    public float dieChanceOverpopulation = 0.6f;
    public int bornValue = 3;
    [Range(0.0f, 1.0f)]
    public float bornChance = 0.6f;

    [Header("Autorun Settings")]
    public int generations = 100;
    public int delay = 1000;
    public bool Autorun = false;

    [Header("Print settings")]
    public float scaleFactor = 1.1f;
    public GameObject prefab;

    private int generation = 0;
    Cell[,] cells;
    private GameObject parent;
    private System.Random rand;

    void Awake()
    {
        if (GameObject.Find("Cells") == null)
            parent = new GameObject("Cells");
        else
            parent = GameObject.Find("Cells");
        populateCells(true);
    }

    public void populateCells(bool random = false, int seed = 0)
    {
        cells = new Cell[columns, rows];
        if (random)
        {
            rand = new System.Random(seed != 0 ? seed : (int)DateTime.Now.Ticks.GetHashCode());
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    cells[i ,j] = new Cell();
                    cells[i, j].Value = rand.Next(0, 2);
                    cells[i, j].x = i;
                    cells[i, j].y = j;
                }
            }
        }
        else
        {
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    cells[i, j].Value = 0;
                }
            }
            cells[columns / 2, rows / 2].Value = 1;
        }
    }

    int computeNeighbors(int x, int y)
    {
        int neighbors = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int _x = x + i > columns - 1 ? 0 : x + i < 0 ? columns - 1 : x + i;
                int _y = y + j > rows - 1 ? 0 : y + j < 0 ? rows - 1 : y + j;
                neighbors += cells[_x, _y].Value;
            }
        }
        neighbors -= cells[x, y].Value;
        return neighbors;
    }

    public void doStep(bool print = true)
    {
        Cell[,] next = new Cell[columns, rows];
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                next[x, y] = new Cell();
                
                int neighbors = computeNeighbors(x, y);

                cells[x, y].neighborsOverGeneration.Add(neighbors);

                if (((cells[x, y].Value) == 1) && (neighbors < lonelinessValue) && (rand.NextDouble() > dieChanceLoneliness)) next[x, y].Value = 0;
                else if (((cells[x, y].Value) == 1) && (neighbors > overpopulationValue) && (rand.NextDouble() > dieChanceOverpopulation)) next[x, y].Value = 0;
                else if (((cells[x, y].Value) == 0) && (neighbors == bornValue) && (rand.NextDouble() > bornChance)) next[x, y].Value = 1;
                else next[x, y] = cells[x, y];
            }
        }
        generation++;
        if (print)
            printGame();
        cells = next;
    }



    public void printGame()
    {
        foreach (Transform go in parent.GetComponentInChildren<Transform>())
        {
            Destroy(go.gameObject);
        }

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject go = Instantiate(prefab, new Vector3(i * scaleFactor, j * scaleFactor, 0), Quaternion.identity, parent.transform);
                if (cells[i, j].Value == 0)
                    go.GetComponent<Renderer>().material.color = Color.black;
                else
                    go.GetComponent<Renderer>().material.color = new Color(cells[i, j].neighborsOverGeneration.Count, cells[i, j].neighborsOverGeneration.Count, cells[i, j].neighborsOverGeneration.Count);
            }
        }
    }
}
