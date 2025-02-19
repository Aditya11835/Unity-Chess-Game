using System.Collections.Generic;
using UnityEngine;

public class RookBehavior : PieceBehavior
{
    private bool hasNotMoved = true;
    public bool HasNotMoved
    {
        get { return hasNotMoved; }
    }
    public void SetHasMoved()
    {
        hasNotMoved = false;
    }

    protected override void hook()
    {
        List<Vector2> legalMoves = GetLegalMoves(oldPos, newPos, false);

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
                if (hasNotMoved)
                {
                    hasNotMoved = false;
                }

                turnFinished = true;
                return;
            }
        }

        // **Invalid move, revert position**
        transform.position = oldPos;
        turnFinished = false;
    }
    public override List<Vector2> GetLegalMoves(Vector2 oldPos, Vector2 newPos, bool isForCheck)
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
}

