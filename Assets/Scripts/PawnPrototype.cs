using System.Collections.Generic;
using UnityEngine;

public class PawnMovement : MonoBehaviour
{
    private PieceSetup pieceSetup;
    private Vector2Int currentPosition;
    private bool isWhite;
    private Vector3 offset;
    private Vector3 originalPosition;
    private const float boardOffset = -3.5f;

    private void Start()
    {
        pieceSetup = FindAnyObjectByType<PieceSetup>();
        currentPosition = GetBoardPosition();
        isWhite = gameObject.name.Contains("White");
    }

    private Vector2Int GetBoardPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x - boardOffset), Mathf.RoundToInt(transform.position.y - boardOffset));
    }

    private Vector3 GetWorldPosition(Vector2Int boardPosition)
    {
        return new Vector3(boardPosition.x + boardOffset, boardPosition.y + boardOffset, 0);
    }

    private void OnMouseDown()
    {
        originalPosition = transform.position;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        newPosition.z = 0;
        transform.position = newPosition;
    }

    private void OnMouseUp()
    {
        Vector2Int newPosition = GetBoardPosition();
        List<Vector2Int> legalMoves = GetLegalMoves();

        if (legalMoves.Contains(newPosition))
        {
            Vector3 worldNewPosition = GetWorldPosition(newPosition);
            Vector3 worldCurrentPosition = GetWorldPosition(currentPosition);

            if (Mathf.Abs(newPosition.x - currentPosition.x) == 1) // Diagonal move (capture)
            {
                if (pieceSetup.pieceDictionary.ContainsKey(worldNewPosition))
                {
                    GameObject capturedPiece = pieceSetup.pieceDictionary[worldNewPosition];
                    bool targetIsWhite = capturedPiece.name.Contains("White");
                    if (targetIsWhite != isWhite) // Ensure only opposite color pieces are captured
                    {
                        pieceSetup.pieceDictionary.Remove(worldNewPosition);
                        Destroy(capturedPiece);
                    }
                }
            }

            pieceSetup.pieceDictionary.Remove(worldCurrentPosition);
            pieceSetup.pieceDictionary[worldNewPosition] = gameObject;
            currentPosition = newPosition;
            transform.position = worldNewPosition;
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    private List<Vector2Int> GetLegalMoves()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int direction = isWhite ? 1 : -1;
        Vector2Int forwardMove = new Vector2Int(currentPosition.x, currentPosition.y + direction);
        Vector3 worldForwardMove = GetWorldPosition(forwardMove);

        if (!pieceSetup.pieceDictionary.ContainsKey(worldForwardMove) || pieceSetup.pieceDictionary[worldForwardMove] == null)
        {
            moves.Add(forwardMove);
            if ((isWhite && currentPosition.y == 1) || (!isWhite && currentPosition.y == 6))
            {
                Vector2Int doubleForwardMove = new Vector2Int(currentPosition.x, currentPosition.y + (2 * direction));
                Vector3 worldDoubleForwardMove = GetWorldPosition(doubleForwardMove);
                if ((!pieceSetup.pieceDictionary.ContainsKey(worldDoubleForwardMove) || pieceSetup.pieceDictionary[worldDoubleForwardMove] == null) &&
                    (!pieceSetup.pieceDictionary.ContainsKey(worldForwardMove) || pieceSetup.pieceDictionary[worldForwardMove] == null))
                {
                    moves.Add(doubleForwardMove);
                }
            }
        }

        Vector2Int[] diagonalMoves =
        {
            new Vector2Int(currentPosition.x - 1, currentPosition.y + direction),
            new Vector2Int(currentPosition.x + 1, currentPosition.y + direction)
        };

        foreach (Vector2Int move in diagonalMoves)
        {
            Vector3 worldDiagonalMove = GetWorldPosition(move);
            if (pieceSetup.pieceDictionary.ContainsKey(worldDiagonalMove) && pieceSetup.pieceDictionary[worldDiagonalMove] != null)
            {
                GameObject targetPiece = pieceSetup.pieceDictionary[worldDiagonalMove];
                bool targetIsWhite = targetPiece.name.Contains("White");
                if (targetIsWhite != isWhite || currentPosition.y == 1 || currentPosition.y == 6)
                {
                    moves.Add(move);
                }
            }
        }
        return moves;
    }
}