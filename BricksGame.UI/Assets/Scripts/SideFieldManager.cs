using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BricksGame.Logic;

public class SideFieldManager : MonoBehaviour
{
    public BrickBehaviour prefab;
    public Side side;

    private int columns, rows;

    private const float TILE_SIZE = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        rows = 3;
        columns = 10;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                //Grid[i, j] = Random.Range(0, 10);
                SpawnTile(i, j);
            }
        }
    }

    private void SpawnTile(int x, int y)
    {
        var g = Instantiate(prefab, gameObject.transform);

        g.color = 3;
        g.x = x; g.y = y;
        g.transform.localPosition = new Vector3(CalcXforLocation(x, y) * TILE_SIZE, CalcYForLocation(x, y) * TILE_SIZE);
    }

    private int CalcXforLocation(int x, int y)
    {
        switch(side)
        {
            case Side.Top:
            case Side.Bottom:
                return x;
            case Side.Left:
                return rows - y - 1;
            case Side.Right:
                return y;
        }

        return x;
    }

    private int CalcYForLocation(int x, int y)
    {
        switch (side)
        {
            case Side.Top:
                return y;
            case Side.Bottom:
                return rows - y - 1;
            case Side.Left:
            case Side.Right:
                return x;
        }
        return y;
    }
}
