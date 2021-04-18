using BricksGame.Logic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Sprite[] _arrows;

    private Direction _direction;
    public Direction Direction 
    { 
        get { return _direction; }
        set
        {
            _direction = value;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sprite = GetArrowSprite();
            }
        }
    }

    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        GameObject g = new GameObject();
        g.transform.parent = this.gameObject.transform;
        g.transform.localPosition = new Vector3(0, 0);

        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = GetArrowSprite();
        _spriteRenderer.sortingOrder = 101;
    }

    private Sprite GetArrowSprite()
    {
        if (_arrows == null || Direction == Direction.None)
            return null;

        var directionIdx = (int)Direction;
        if (_arrows.Length < directionIdx)
            return null;

        return _arrows[directionIdx - 1];
    }
}
