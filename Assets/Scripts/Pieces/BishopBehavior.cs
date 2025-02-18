using System.Collections.Generic;
using UnityEngine;

public class BishopBehavior : PieceBehavior
{
    protected override List<Vector2> GetLegalMoves(Vector2 oldPos, Vector2 newPos)
    {
        List<Vector2> legalMoves = new List<Vector2>();
        if (pieceSetup.pieceDictionary == null) return legalMoves;

        // **Define Bishop's Four Directions**
        Vector2[] directions = new Vector2[]
        {
        new Vector2( 1,  1),  // Diagonal Up Right
        new Vector2(-1,  1),  // Diagonal Up Left
        new Vector2( 1,  -1),  // Diagonal Down Right
        new Vector2(-1, -1)   // Diagonal Down Left
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

