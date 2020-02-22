using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    Vector2 startDragPosition;
    Vector2? targetPosition;
    public SwapEvent SwapEvent;
    public (int x, int y) GridPosition { get; set; }
    public bool Visited { get; set; }
    public bool ToBeDestroyed { get; set; }

    // Use this for initialization
    void Start()
    {
        Visited = false;
        ToBeDestroyed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ToBeDestroyed)
        {
            var c = this.GetComponent<SpriteRenderer>().color;
            c.a = 0.5f;
            this.GetComponent<SpriteRenderer>().color = c;

        }
        if (targetPosition == null)
        {
            return;
        }
        var t = targetPosition.Value;
        if (Vector2.Distance(transform.localPosition, t) > 0.1)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, t, 3.0f * Time.deltaTime);
        }
        else
        {
            transform.localPosition = t;
            targetPosition = null;
        }
    }

    public void OnMouseDown()
    {
        startDragPosition = Input.mousePosition;
    }

    public void OnMouseUp()
    {
        var endDragPosition = Input.mousePosition;
        var angle = Mathf.Atan2(endDragPosition.y - startDragPosition.y, endDragPosition.x - startDragPosition.x) * Mathf.Rad2Deg;

        HandleSwap(angle);
    }

    public string ToPosString()
    {
        return "(" + this.transform.localPosition.x + ", " + this.transform.localPosition.y + ")";
    }

    private void HandleSwap(float angle)
    {
        Board.SwapDirection direction;
        if (angle > -45 && angle <= 45)
        {
            // Right
            direction = Board.SwapDirection.RIGHT;
        }
        else if (angle > 45 && angle <= 135)
        {
            // Up
            direction = Board.SwapDirection.UP;
        }
        else if (angle > -135 && angle <= -45)
        {
            // Down
            direction = Board.SwapDirection.DOWN;
        }
        else
        {
            // Left
            direction = Board.SwapDirection.LEFT;
        }
        SwapEvent.Invoke(this.gameObject, direction);
    }

    internal void MoveTo((int x, int y) newPosition)
    {
        var (x, y) = newPosition;
        GridPosition = (x, y);
        targetPosition = new Vector2(x, y);
    }
}



[System.Serializable]
public class SwapEvent : UnityEvent<GameObject, Board.SwapDirection>
{

}
