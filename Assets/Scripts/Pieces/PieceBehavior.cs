using System.Collections.Generic;
using UnityEngine;

public abstract class PieceBehavior : MonoBehaviour
{
    public bool isWhite;
    protected Camera cam;
    protected float tileSize = 1.0f;
    protected Vector2 boardOffset = new Vector2(-3.5f, -3.5f);
    protected Vector2 oldPos;
    protected Vector2 newPos;
    protected PieceSetup pieceSetup;
    protected Vector3 cursorOffset;
    protected bool turnFinished = false;

    public abstract List<Vector2> GetLegalMoves(Vector2 oldPos, Vector2 newPos);
    protected virtual bool IsCapture(Vector2 oldPos, Vector2 newPos)
    {
        // **Ensure the target position has a piece**
        if (!pieceSetup.pieceDictionary.ContainsKey(newPos))
        {
            return false; // No piece to capture
        }

        GameObject targetPiece = pieceSetup.pieceDictionary[newPos];

        // **Ensure the target piece has PieceBehavior (prevents errors)**
        PieceBehavior targetPieceBehavior = targetPiece.GetComponent<PieceBehavior>();
        if (targetPieceBehavior == null)
        {
            return false; // Not a valid piece
        }

        // **Check if it's an opponent piece**
        return targetPieceBehavior.isWhite != isWhite;
    }

    protected bool IsWithinBoard(Vector2 position)
    {
        return position.x >= -4.0f && position.x <= 4.0f && position.y >= -4.0f && position.y <= 4.0f;
    }


    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0f;
        return cam.ScreenToWorldPoint(mousePoint);
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
    protected virtual void OnMouseDown()
    {
        oldPos = transform.position;
        cursorOffset = transform.position - GetMouseWorldPos(); //Difference between cursor position and center of sprite
    }
    protected virtual void OnMouseDrag()
    {
        // Ensure only the correct player's pieces can move
        if ((isWhite && !TurnManager.Instance.IsWhiteTurn()) || (!isWhite && TurnManager.Instance.IsWhiteTurn()))
        {
            Debug.Log("It's not " + (isWhite ? "White's" : "Black's") + " turn.");
            return; // Do nothing if it's not this piece's turn
        }
        transform.position = GetMouseWorldPos() + cursorOffset; //Maintains the difference so sprite doesn't snap to cursor
    }
    protected virtual void OnMouseUp()
    {
        turnFinished = false;
        newPos = transform.position;
        if (newPos.x > 4 || newPos.x < -4 || newPos.y > 4 || newPos.y < -4)
        {
            transform.position = oldPos;
            return;
        }
        float snappedX = Mathf.Round((newPos.x - boardOffset.x) / tileSize) * tileSize + boardOffset.x;
        float snappedY = Mathf.Round((newPos.y - boardOffset.y) / tileSize) * tileSize + boardOffset.y;
        newPos = new Vector2(snappedX, snappedY);
        if (newPos == oldPos)
        {
            transform.position = oldPos;
            return;
        }
        hook();
        if (turnFinished)
        {
            KeyValuePair<Vector2, Vector2> currentMove = new KeyValuePair<Vector2, Vector2>(oldPos, newPos);
            TurnManager.Instance.SetLastMove(currentMove);
            TurnManager.Instance.SwitchTurn();
        }else
        {
            return;
        }
    }
    protected virtual void hook() {
        List<Vector2> legalMoves = GetLegalMoves(oldPos, newPos);

        foreach (Vector2 legalMove in legalMoves)
        {
            if (newPos == legalMove)
            {
                // **Check if the move is a capture**
                if (IsCapture(oldPos, newPos))
                {
                    GameObject targetPiece = pieceSetup.pieceDictionary[newPos];
                    pieceSetup.pieceDictionary.Remove(newPos); //  Remove from dictionary first
                    Destroy(targetPiece); //  Then destroy the object
                }

                // **Move the piece in dictionary**
                pieceSetup.pieceDictionary.Remove(oldPos);
                pieceSetup.pieceDictionary[newPos] = gameObject;
                transform.position = newPos;

                turnFinished = true;
                return;
            }
        }

        // **Invalid move, revert position**
        transform.position = oldPos;
        turnFinished = false;
    }
}

