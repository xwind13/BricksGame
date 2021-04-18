using BricksGame.Logic;
using BricksGame.Logic.Models;
using System;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private Sprite[] _colors;

    public ISquare Value { get; set; }

    public event Action<ISquare> BrickPressed;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (Value == null)
            return;

        GameObject g = new GameObject();
        g.transform.parent = this.gameObject.transform;
        g.transform.localPosition = new Vector3(0, 0);

        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = GetColorSprite(Value.Color);
        _spriteRenderer.sortingOrder = Value is MovingSquare ? 99 : 10;

        Value.StateChanged += Value_StateChanged;

        if (Value is IMainFieldSquare)
        {
            this.enabled = (Value as IMainFieldSquare).State.IsActive;
        }
    }

    private void Value_StateChanged(ISquare square)
    {
        _spriteRenderer.sprite = GetColorSprite(square.Color);
        if (square is IMainFieldSquare)
        {
            this.enabled = (square as IMainFieldSquare).State.IsActive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick();
    }

    private void HandleClick()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var rayHit = Physics2D.Raycast(pos, Vector2.zero);
        if (rayHit.transform == null)
            return;

        // only first row can handle click.
        if (rayHit.transform == this.transform)
        {
            BrickPressed?.Invoke(Value);
        }
    }

    private Sprite GetColorSprite(BricksGame.Logic.Color color)
    {
        if (_colors == null || color == BricksGame.Logic.Color.None)
            return null;

        var colorIdx = (int)color;
        if (_colors.Length < colorIdx)
            return null;

        return _colors[colorIdx - 1];
        
    }
}
