using System;
using Unity.VisualScripting;
using UnityEngine;

public class PawnBeha : PieceBehavior
{
    private bool enPassant;
    private int enPassantCounter;
    private bool canPromote;
    public bool getEnPassant()
    {
        return this.enPassant;
    }
    override protected bool IsCapture(Vector2 oldPos, Vector2 newPos)
    {
        int direction = isWhite ? 1 : -1;
        if (newPos == oldPos + new Vector2(1, direction) || newPos == oldPos + new Vector2(-1, direction))
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
                PawnBeha adjacentPawnBehavior = adjacentPawn.GetComponent<PawnBeha>();
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
    override protected bool IsLegalMove(Vector2 oldPos, Vector2 newPos)
    {
        int direction = isWhite ? 1 : -1;
        Vector2 forwardMove = new Vector2(oldPos.x, oldPos.y + direction);
        Vector2 doubleForwardMove = new Vector2(oldPos.x, oldPos.y + (2 * direction));
        if (newPos == forwardMove && !pieceSetup.pieceDictionary.ContainsKey(forwardMove))
        {
            return true;
        }
        if (newPos == doubleForwardMove && (oldPos.y == -2.5 && isWhite) || (oldPos.y == 2.5 && !isWhite) && !pieceSetup.pieceDictionary.ContainsKey(doubleForwardMove) && !pieceSetup.pieceDictionary.ContainsKey(forwardMove))
        {
            enPassant = true;
            enPassantCounter = 1;
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
            if (transform.position.y == -3.5)
            {
                canPromote = true;
            }
        }
    }
    override protected void Start()
    {
        base.Start();
        canPromote = false;
    }
    override protected void OnMouseUp()
    {
        base.OnMouseUp();
        if (enPassantCounter > 0)
        {
            enPassantCounter--;
            if (enPassantCounter == 0)
            {
                foreach (var entry in pieceSetup.pieceDictionary)
                {
                    if (entry.Value.TryGetComponent<PawnBeha>(out var pawn) &&
                        pawn.transform.position.y == (pawn.isWhite ? -0.5f : 0.5f))
                    {
                        pawn.enPassant = false;
                    }
                }
            }
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
                //GameObject piecePrefab = isWhite ? pieceSetup.piecePrefabs[8] : pieceSetup.piecePrefabs[9];
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