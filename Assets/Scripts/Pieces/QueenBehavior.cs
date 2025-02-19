using System.Collections.Generic;
using UnityEngine;

public class QueenBehavior : PieceBehavior
{
    public override List<Vector2> GetLegalMoves(Vector2 oldPos, Vector2 newPos)
    {
        List<Vector2> legalMoves = new List<Vector2>();
        if (pieceSetup.pieceDictionary == null) return legalMoves;

        // **Define Queen's Eight Directions**
        Vector2[] directions = new Vector2[]
        {
        new Vector2( 1,  1),  // Diagonal Up Right
        new Vector2(-1,  1),  // Diagonal Up Left
        new Vector2( 1,  -1),  // Diagonal Down Right
        new Vector2(-1, -1),   // Diagonal Down Left
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
                    break; // **Stop moving further in this direction (queen can't jump)**
                }

                // **Empty square, add as legal move**
                legalMoves.Add(nextPos);
                nextPos += dir; // Move further in the same direction
            }
        }

        return legalMoves;
    }
}
