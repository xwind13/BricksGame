using Assets.Scripts;
using BricksGame.Logic.Models;
using UnityEngine;

public class MovingBrick : MonoBehaviour
{
    [SerializeField] private Brick _prefab;

    public MovingSquare Value { get; set; }

    private Brick _movingBrick;

    private const float Step = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        if (Value == null)
            return;

        Value.StateChanged += MovingSquare_StateChanged;
        SpawnTile();
    }

    private void MovingSquare_StateChanged(BricksGame.Logic.ISquare state)
    {
        var movingSquare = state as MovingSquare;
        if (movingSquare == null)
            return;

        _movingBrick.HideShow(movingSquare.IsMoving);
        _movingBrick.transform.localPosition = new Vector3(
            movingSquare.X * BrickSetting.TileSize, 
            movingSquare.Y * BrickSetting.TileSize, 1);
    }

    private void SpawnTile()
    {
        _movingBrick = Instantiate(_prefab, gameObject.transform);

        _movingBrick.Value = Value;
        _movingBrick.transform.localPosition = new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (Value != null && Value.IsMoving)
        {
            if (Value.Direction == BricksGame.Logic.Direction.None)
                FinishMove();

            _movingBrick.transform.localPosition += GetMoveStep();

            if (IsDestinationReached())
                FinishMove();
        }
    }

    private Vector3 GetMoveStep() => Value.Direction switch
    {
        BricksGame.Logic.Direction.Right => new Vector3(Step, 0, 0),
        BricksGame.Logic.Direction.Left => new Vector3(-Step, 0, 0),
        BricksGame.Logic.Direction.Up => new Vector3(0, Step, 0),
        BricksGame.Logic.Direction.Down => new Vector3(0, -Step, 0),
        _ => new Vector3(0, 0, 0)
    };

    private bool IsDestinationReached() => Value.Direction switch
    {
        BricksGame.Logic.Direction.Right => _movingBrick.transform.localPosition.x >= Value.Destination * BrickSetting.TileSize,
        BricksGame.Logic.Direction.Left => _movingBrick.transform.localPosition.x <= Value.Destination * BrickSetting.TileSize,
        BricksGame.Logic.Direction.Up => _movingBrick.transform.localPosition.y >= Value.Destination * BrickSetting.TileSize,
        BricksGame.Logic.Direction.Down => _movingBrick.transform.localPosition.y <= Value.Destination * BrickSetting.TileSize,
        _ => true
    };
    

    private void FinishMove()
    {
        _movingBrick.HideShow(false);
        Value.Finish();
    }
}
