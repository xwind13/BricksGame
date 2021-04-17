using Assets.Scripts;
using UnityEngine;

public class GameField : MonoBehaviour
{
    [SerializeField] private SideField _sideFieldPrefub;
    [SerializeField] private MainField _mainFieldPrefub;

    private const float Offset = BrickSetting.TileSize / 2;
    private const int HalfFieldSize = 5;

    // Start is called before the first frame update
    void Start()
    {
        InstantiateMainField();

        InstantiateTopSideField();
        InstantiateLeftSideField();
        InstantiateBottomSideField();
        InstantiateRightSideField();
    }

    private SideField InstantiateTopSideField()
    {
        var position = new Vector3(
            -HalfFieldSize * BrickSetting.TileSize + Offset,
            HalfFieldSize * BrickSetting.TileSize + Offset, 0);

        return InstantiateSideField(BricksGame.Logic.Side.Top, position);
    }

    private SideField InstantiateLeftSideField()
    {
        var position = new Vector3(
            -HalfFieldSize * BrickSetting.TileSize - BrickSetting.SideFieldSize * BrickSetting.TileSize + Offset,
            -HalfFieldSize * BrickSetting.TileSize + Offset, 0);

        return InstantiateSideField(BricksGame.Logic.Side.Left, position);
    }

    private SideField InstantiateBottomSideField()
    {
        var position = new Vector3(
            -HalfFieldSize * BrickSetting.TileSize + Offset,
            -HalfFieldSize * BrickSetting.TileSize - BrickSetting.SideFieldSize * BrickSetting.TileSize + Offset, 0);

        return InstantiateSideField(BricksGame.Logic.Side.Bottom, position);
    }

    private SideField InstantiateRightSideField()
    {
        var position = new Vector3(
            HalfFieldSize * BrickSetting.TileSize + Offset,
            -HalfFieldSize * BrickSetting.TileSize + Offset, 0);

        return InstantiateSideField(BricksGame.Logic.Side.Right, position);
    }

    private SideField InstantiateSideField(BricksGame.Logic.Side side, Vector3 position)
    {
        var sideField = Instantiate(_sideFieldPrefub, gameObject.transform);
        sideField.Side = side;
        sideField.transform.position = position;

        return sideField;
    }

    private MainField InstantiateMainField()
    {
        var mainField = Instantiate(_mainFieldPrefub, gameObject.transform);
        mainField.transform.position = new Vector3(
            -HalfFieldSize * BrickSetting.TileSize + Offset, 
            -HalfFieldSize * BrickSetting.TileSize + Offset, 0);

        return mainField;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
