using UnityEngine;
using BricksGame.Logic;
using Assets.Scripts;

public class SideField : MonoBehaviour
{
    [SerializeField] private Brick _prefab;
    [SerializeField] public Side Side { get; set; }

    private int columns, rows;

    // Start is called before the first frame update
    void Start()
    {
        rows = 3;
        columns = 10;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                SpawnTile(i, j);
            }
        }
    }

    private void SpawnTile(int x, int y)
    {
        var g = Instantiate(_prefab, gameObject.transform);

        g.Color = 3;
        g.SetRelativeCoords(x, y);
        g.transform.localPosition = new Vector3(
            CalcXforLocation(x, y) * BrickSetting.TileSize, 
            CalcYForLocation(x, y) * BrickSetting.TileSize);
    }

    private int CalcXforLocation(int x, int y)
    {
        switch(Side)
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
        switch (Side)
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
