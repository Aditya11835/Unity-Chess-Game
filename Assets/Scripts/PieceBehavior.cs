using UnityEngine;

public abstract class PieceBehavior : MonoBehaviour
{
    protected bool isWhite;
    protected Camera cam;
    protected float tileSize = 1.0f;
    protected Vector2 boardOffset = new Vector2(-3.5f, -3.5f);
    protected Vector3 cursorOffset;
    protected Vector2 oldPos;
    protected Vector2 newPos;
    protected PieceSetup pieceSetup;

    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0f;
        return cam.ScreenToWorldPoint(mousePoint);
    }
    protected bool IsTurn()
    {
        if ((isWhite && !TurnManager.Instance.IsWhiteTurn()) || (!isWhite && TurnManager.Instance.IsWhiteTurn()))
        {
            Debug.Log("It's not " + (isWhite ? "White's" : "Black's") + " turn.");
            return false;
        }
        return true;
    }
    protected virtual void OnMouseDown()
    {
        oldPos = transform.position;
        cursorOffset = transform.position - GetMouseWorldPos(); //Difference between cursor position and center of sprite
        if (!IsTurn())
        {
            return;
        }
    }
    protected virtual void OnMouseDrag()
    {
        if (!IsTurn()) 
        {
            return;
        }
        transform.position = GetMouseWorldPos() + cursorOffset; //Maintains the difference so sprite doesn't snap to cursor
    }
    protected virtual void OnMouseUp()
    {
        if (!IsTurn())
        {
            return;
        }
        newPos = GetMouseWorldPos() + cursorOffset;
        if (newPos.x > 4 || newPos.x < -4 || newPos.y > 4 || newPos.y < -4)
        {
            transform.position = oldPos;
            return;
        }
        float snappedX = Mathf.Round((newPos.x - boardOffset.x) / tileSize) * tileSize + boardOffset.x;
        float snappedY = Mathf.Round((newPos.y - boardOffset.y) / tileSize) * tileSize + boardOffset.y;
        newPos = new Vector3(snappedX, snappedY, 0);
    }
    
    protected virtual void Start()
    {
        cam = Camera.main;
        pieceSetup = FindAnyObjectByType<PieceSetup>();
        foreach (var entry in pieceSetup.pieceDictionary)
        {
            if (entry.Value == gameObject) // If this GameObject matches the dictionary entry
            {
                oldPos = entry.Key; // Get the corresponding position
                break;
            }
        }
        isWhite = gameObject.name.Contains("White");
    }
    protected abstract bool IsCapture(Vector2 oldPos, Vector2 newPos);
    protected abstract bool IsLegalMove(Vector2 oldPos, Vector2 newPos);


}
