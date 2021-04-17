using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainField : MonoBehaviour
{
    [SerializeField] private Brick _brick;
    [SerializeField] private MovingBrick _movingBrick;

    private int columns, rows;

    // Start is called before the first frame update
    void Start()
    {
        rows = 10;
        columns = 10;

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                //Grid[i, j] = Random.Range(0, 10);
                SpawnTile(i, j);
            }
        }

        Instantiate(_movingBrick, gameObject.transform);
    }

    private void SpawnTile(int x, int y)
    {
        var g = Instantiate(_brick, gameObject.transform);

        g.Color = 3;
        g.SetRelativeCoords(x, y);
        g.transform.localPosition = new Vector3(
            x * BrickSetting.TileSize,
            y * BrickSetting.TileSize);

        g.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
