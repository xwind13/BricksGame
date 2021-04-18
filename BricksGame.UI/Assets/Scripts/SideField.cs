using UnityEngine;
using BricksGame.Logic;
using Assets.Scripts;
using BricksGame.Logic.Matrix;
using System.ComponentModel;
using System;
using Assets.Scripts.Events;

public class SideField : MonoBehaviour
{
    [SerializeField] private Brick _prefab;

    public Side Side { get; set; }
    public IMatrix<ISquare> Matrix { get; set; }

    public event EventHandler<SideFieldClickEventArgs> ClickEventHandler;

    // Start is called before the first frame update
    void Start()
    {
        if (Matrix == null)
            return;

        Matrix.ForEach(square => SpawnBrick(square));
    }

    private void SpawnBrick(ISquare square)
    {
        var g = Instantiate(_prefab, gameObject.transform);

        g.Value = square;
        g.transform.localPosition = new Vector3(
            CalcXforLocation(square.X, square.Y) * BrickSetting.TileSize, 
            CalcYForLocation(square.X, square.Y) * BrickSetting.TileSize);

        g.BrickPressed += BrickPressed;
    }

    private void BrickPressed(ISquare sqare)
    {
        if (sqare.Y == 0)
        {
            ClickEventHandler?.Invoke(this, new SideFieldClickEventArgs() { Side = Side, PosIdx = sqare.X });
        }
    }

    private uint CalcXforLocation(uint x, uint y) => Side switch
    {
        Side.Top => x,
        Side.Bottom => x,
        Side.Left => Matrix.Height - y - 1,
        Side.Right => y,

        _ => throw new InvalidEnumArgumentException($"{Side} is invalid argument")
    };

    private uint CalcYForLocation(uint x, uint y) => Side switch
    {
        Side.Top => y,
        Side.Bottom => Matrix.Height - y - 1,
        Side.Left => x,
        Side.Right => x,

        _ => throw new InvalidEnumArgumentException($"{Side} is invalid argument")
    };
}
