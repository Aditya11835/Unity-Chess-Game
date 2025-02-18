using System.Collections.Generic;
using UnityEngine;

public class RookBehavior : PieceBehavior
{
    bool hasMoved = false;

    protected override void hook()
    {
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
                if (!hasMoved)
                {
                    hasMoved = !hasMoved;
                }

                turnFinished = true;
                return;
            }
        }

        // **Invalid move, revert position**
        transform.position = oldPos;
        turnFinished = false;
    }
    protected override List<Vector2> GetLegalMoves(Vector2 oldPos, Vector2 newPos)
    {
        List<Vector2> legalMoves = new List<Vector2>();
        if (pieceSetup.pieceDictionary == null) return legalMoves;

        // **Define Rook's Four Directions**
        Vector2[] directions = new Vector2[]
        {
        new Vector2( 1,  0),  // Right (+X)
        new Vector2(-1,  0),  // Left (-X)
        new Vector2( 0,  1),  // Up (+Y)
        new Vector2( 0, -1)   // Down (-Y)
        };

        // **Iterate Over Each Direction**
        foreach (Vector2 dir in directions)
        {
            Vector2 nextPos = oldPos + dir;  // Move one step in the direction

            while (IsWithinBoard(nextPos))
            {
                // **Check if the square is occupied**
                if (pieceSetup.pieceDictionary.ContainsKey(nextPos))
                {
                    // **If occupied by an opponent, it's a valid capture move**
                    if (IsCapture(oldPos, nextPos))
                    {
                        legalMoves.Add(nextPos);
                    }
                    break; // **Stop moving further in this direction (rook can't jump)**
                }

                // **Empty square, add as legal move**
                legalMoves.Add(nextPos);
                nextPos += dir; // Move further in the same direction
            }
        }

        return legalMoves;
    }
    protected override bool IsCapture(Vector2 oldPos, Vector2 newPos)
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
}

