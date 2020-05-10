using System.Collections.Generic;

public class Cell
{
    public int x = 0;
    public int y = 0;

    private int value = 0;

    public List<int> neighborsOverGeneration = new List<int>();

    public int Value { get => value; set {this.value = value; neighborsOverGeneration.Clear();}}

    public Cell(){}
}
