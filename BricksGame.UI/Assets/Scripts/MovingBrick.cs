using Assets.Scripts;
using UnityEngine;

public class MovingBrick : MonoBehaviour
{
    [SerializeField] private Brick _prefab;

    private int _endPoint = 4;
    private Brick _movingBrick; 

    // Start is called before the first frame update
    void Start()
    {
        SpawnTile();
    }

    private void SpawnTile()
    {
        _movingBrick = Instantiate(_prefab, gameObject.transform);

        _movingBrick.Color = 3;
        _movingBrick.IsTop = true;
        _movingBrick.transform.localPosition = new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }


    private void Move()
    {
        if (_movingBrick.transform.localPosition.x <= _endPoint * BrickSetting.TileSize)
        {
            _movingBrick.transform.position += new Vector3(0.1f, 0, 0);
        }
    }
}
