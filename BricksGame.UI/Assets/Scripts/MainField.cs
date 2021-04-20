using Assets.Scripts;
using BricksGame.Logic;
using BricksGame.Logic.Matrix;
using BricksGame.Logic.Models;
using UnityEngine;

public class MainField : MonoBehaviour
{
    [SerializeField] private Brick _brick;
    [SerializeField] private MovingBrick _movingBrick;

    public IMatrix<IMainFieldSquare> Matrix { get; set; }
    public MovingSquare MovingSquare { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        if (Matrix == null)
            return;

        Matrix.ForEach(square => SpawnBrick(square));
        InstantiateMovingSquare();
    }

    private void InstantiateMovingSquare()
    {
        var g = Instantiate(_movingBrick, gameObject.transform);
        g.Value = MovingSquare;
    }

    private void SpawnBrick(IMainFieldSquare square)
    {
        var g = Instantiate(_brick, gameObject.transform);

        g.Value = square;
        g.transform.localPosition = new Vector3(
            square.X * BrickSetting.TileSize,
            square.Y * BrickSetting.TileSize);
    }
}
