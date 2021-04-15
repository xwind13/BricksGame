using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBehaviour : MonoBehaviour
{
    public Sprite[] colors;
    public int color;


    // coords of object.
    public int x;
    public int y;

    private string brickName;

    // Start is called before the first frame update
    void Start()
    {
        brickName = $"x: {x}, y: {y}";
        GameObject g = new GameObject(brickName);
        g.transform.parent = this.gameObject.transform;
        g.transform.localPosition = new Vector3(0, 0);
        var s = g.AddComponent<SpriteRenderer>();
        s.sprite = colors[color];
        s.sortingOrder = 10;
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

        if (rayHit.transform == this.transform && y == 0)
        {
           print(brickName);
        }
        
    }
}
