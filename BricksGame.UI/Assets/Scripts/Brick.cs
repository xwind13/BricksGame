using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private Sprite[] _colors;

    public int Color { get; set; }
    public bool IsTop { get; set; } = false;

    // relative coords for brick in field.
    private int _x;
    private int _y;

    private string _brickName;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _brickName = $"x: {_x}, y: {_y}";
        GameObject g = new GameObject(_brickName);
        g.transform.parent = this.gameObject.transform;
        g.transform.localPosition = new Vector3(0, 0);

        _spriteRenderer = g.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _colors[Color];
        _spriteRenderer.sortingOrder = IsTop ? 99 : 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            HandleClick();
    }

    private void HandleClick()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var rayHit = Physics2D.Raycast(pos, Vector2.zero);
        if (rayHit.transform == null)
            return;

        // only first row can handle click.
        if (rayHit.transform == this.transform && _y == 0)
        {
           print(_brickName);
        }
    }

    public void SetRelativeCoords(int x, int y)
    {
        _x = x;
        _y = y;
    }
}
