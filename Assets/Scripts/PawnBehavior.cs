using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class PawnBehavior : MonoBehaviour
{
    private bool isWhite;
    private bool enPassant;
    private bool canPromote;
    private Camera cam;
    private float tileSize = 1.0f;
    private Vector2 boardOffset = new Vector2(-3.5f, -3.5f);
    private Vector2 oldPos;
    private Vector2 newPos;
    private PieceSetup pieceSetup;
    private Vector3 cursorOffset;

    public bool getEnPassant()
    {
        return this.enPassant;
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0f;
        return cam.ScreenToWorldPoint(mousePoint);
    }
    bool IsCapture(Vector2 oldPos, Vector2 newPos)
    {
        int direction = isWhite ? 1 : -1;
        if (newPos == oldPos + new Vector2(1, direction) || newPos == oldPos + new Vector2(-1,direction))
        {
            if (pieceSetup.pieceDictionary.ContainsKey(newPos))
            {
                GameObject occupyingPiece = pieceSetup.pieceDictionary[newPos];
                bool IsWhite = occupyingPiece.name.Contains("White");
                if (IsWhite != isWhite)
                {
                    //GameObject capturedPiece = pieceSetup.pieceDictionary[newPos];
                    pieceSetup.pieceDictionary.Remove(newPos);
                    Destroy(occupyingPiece);
                    return true;
                }
            }
            Vector2 enPassantTarget = new Vector2(newPos.x, newPos.y - direction);
            if (pieceSetup.pieceDictionary.ContainsKey(enPassantTarget))
            {
                GameObject adjacentPawn = pieceSetup.pieceDictionary[enPassantTarget];
                PawnBehavior adjacentPawnBehavior = adjacentPawn.GetComponent<PawnBehavior>();
                if (adjacentPawnBehavior != null && adjacentPawnBehavior.getEnPassant() && adjacentPawn.name.Contains(isWhite ? "Black" : "White"))
                {
                    pieceSetup.pieceDictionary.Remove(enPassantTarget);
                    Destroy(adjacentPawn);
                    return true; // En passant capture
                }
            }
        }
        return false;
    }
    bool IsLegalMove(Vector2 oldPos, Vector2 newPos)
    {
        int direction = isWhite ? 1 : -1;
        Vector2 forwardMove = new Vector2(oldPos.x, oldPos.y + direction);
        Vector2 doubleForwardMove = new Vector2(oldPos.x, oldPos.y + (2 * direction));
        if (newPos == forwardMove && !pieceSetup.pieceDictionary.ContainsKey(forwardMove))
        {
            return true;
        }
        if(newPos == doubleForwardMove && (oldPos.y == -2.5 && isWhite) || (oldPos.y == 2.5 && !isWhite) && !pieceSetup.pieceDictionary.ContainsKey(doubleForwardMove) && !pieceSetup.pieceDictionary.ContainsKey(forwardMove))
        {
            enPassant = true;
            return true;
        }
        if (IsCapture(oldPos, newPos)) 
        {
            return true;
        }
        return false;
    }
    void checkPromote()
    {
        if (isWhite)
        {
            if (transform.position.y == 3.5)
            {
                canPromote = true;
            }
        }
        else
        {
            if(transform.position.y == -3.5)
            {
                canPromote = true;
            }
        }
    }
    void Start()
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
        canPromote = false;
    }
    private void OnMouseDown()
    {
        oldPos = transform.position;
        cursorOffset = transform.position - GetMouseWorldPos(); //Difference between cursor position and center of sprite
        // Ensure only the correct player's pieces can move
        if ((isWhite && !TurnManager.Instance.IsWhiteTurn()) || (!isWhite && TurnManager.Instance.IsWhiteTurn()))
        {
            Debug.Log("It's not " + (isWhite ? "White's" : "Black's") + " turn.");
            return; // Do nothing if it's not this piece's turn
        }
    }
    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + cursorOffset; //Maintains the difference so sprite doesn't snap to cursor
    }
    private void OnMouseUp()
    {
        // Ensure only the correct player's pieces can move
        if ((isWhite && !TurnManager.Instance.IsWhiteTurn()) || (!isWhite && TurnManager.Instance.IsWhiteTurn()))
        {
            Debug.Log("It's not " + (isWhite ? "White's" : "Black's") + " turn.");
            transform.position = oldPos; // Reset position to prevent movement
            return;
        }
        newPos = GetMouseWorldPos()+cursorOffset;
        if (newPos.x > 4 || newPos.x < -4 || newPos.y > 4 || newPos.y < -4)
        {
            transform.position = oldPos;
            return;
        }
        float snappedX = Mathf.Round((newPos.x - boardOffset.x) / tileSize) * tileSize + boardOffset.x;
        float snappedY = Mathf.Round((newPos.y - boardOffset.y) / tileSize) * tileSize + boardOffset.y;
        newPos = new Vector3(snappedX, snappedY, 0);
        if(newPos == oldPos)
        {
            transform.position = oldPos;
            return;
        }
        if (IsLegalMove(oldPos, newPos))
        {
            if (!IsCapture(oldPos, newPos))
            {
                pieceSetup.pieceDictionary.Remove(oldPos);
                pieceSetup.pieceDictionary[newPos] = gameObject;
                transform.position = newPos;
            }
            else
            {
                pieceSetup.pieceDictionary.Remove(oldPos);
                pieceSetup.pieceDictionary[(newPos)] = gameObject;
                transform.position = newPos;
            }
            if (!canPromote)
            {
                checkPromote();
            }
            if (canPromote)
            {
                Vector2 tempPos = gameObject.transform.position;
                Transform pP = pieceSetup.piecesParent;
                String name = gameObject.name.Split('_')[1];
                Destroy(gameObject);
                GameObject piece = isWhite ? Instantiate(pieceSetup.piecePrefabs[8], tempPos, Quaternion.identity, pP) : Instantiate(pieceSetup.piecePrefabs[9], tempPos, Quaternion.identity, pP);
                piece.name = $"Promoted_{piece.name.Split('(')[0]}_{name}";
                piece.transform.localScale = new Vector3(4, 4, 0);
                pieceSetup.pieceDictionary[piece.transform.position] = piece;
            }
            // Switch turn after a successful move
            TurnManager.Instance.SwitchTurn();
        }
        else
        {
            transform.position = oldPos;
        }

    }
}
