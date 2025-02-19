using System.Collections.Generic;
using UnityEngine;

public class KingBehavior : PieceBehavior
{
    private bool hasNotMoved = true; // Track whether the king has moved

    protected override List<Vector2> GetLegalMoves(Vector2 oldPos, Vector2 newPos)
    {
        List<Vector2> legalMoves = new List<Vector2>();

        // Define all possible king move offsets (one square in any direction)
        Vector2[] moveOffsets = new Vector2[]
        {
            new Vector2( 1,  0), new Vector2(-1,  0), // Left, Right
            new Vector2( 0,  1), new Vector2( 0, -1), // Up, Down
            new Vector2( 1,  1), new Vector2( 1, -1), // Diagonal 
            new Vector2(-1,  1), new Vector2(-1, -1)  // Diagonal 
        };

        // Iterate through all possible moves
        foreach (Vector2 offset in moveOffsets)
        {
            Vector2 potentialMove = oldPos + offset;

            // Ensure the move is within the board boundaries
            if (IsWithinBoard(potentialMove))
            {
                // If square is empty or contains an opponent, it's a legal move
                if (!pieceSetup.pieceDictionary.ContainsKey(potentialMove) || IsCapture(oldPos, potentialMove))
                {
                    legalMoves.Add(potentialMove);
                }
            }
        }

        // **Check for Castling**
        if (hasNotMoved)
        {
            legalMoves.AddRange(GetCastlingMoves(oldPos));
        }
        Debug.Log($"King Legal Moves at {oldPos}: {legalMoves.Count}");
        return legalMoves;
    }

    // ** Castling Logic**
    private List<Vector2> GetCastlingMoves(Vector2 kingPos)
    {
        List<Vector2> castlingMoves = new List<Vector2>();

        // **Find possible rook positions for castling**
        Vector2 kingsideRookPos = new Vector2(kingPos.x + 3, kingPos.y);
        Vector2 queensideRookPos = new Vector2(kingPos.x - 4, kingPos.y);

        // **Check if castling is possible on either side**
        if (CanCastle(kingPos, kingsideRookPos)) // Kingside
        {
            castlingMoves.Add(new Vector2(kingPos.x + 2, kingPos.y));
        }
        if (CanCastle(kingPos, queensideRookPos)) // Queenside
        {
            castlingMoves.Add(new Vector2(kingPos.x - 2, kingPos.y));
        }

        return castlingMoves;
    }

    private bool CanCastle(Vector2 kingPos, Vector2 rookPos)
    {
        // **Ensure the rook exists at the expected position**
        if (!pieceSetup.pieceDictionary.ContainsKey(rookPos))
            return false;

        GameObject rook = pieceSetup.pieceDictionary[rookPos];
        RookBehavior rookBehavior = rook.GetComponent<RookBehavior>();

        // **Ensure it's a rook and it hasn't moved**
        if (rookBehavior == null || !rookBehavior.HasNotMoved)
            return false;

        // **Ensure there are no pieces between the king and rook**
        Vector2 step = (rookPos.x > kingPos.x) ? Vector2.right : Vector2.left; // Determine direction
        Vector2 currentPos = kingPos + step;

        while (currentPos != rookPos)
        {
            if (pieceSetup.pieceDictionary.ContainsKey(currentPos))
                return false; // Blocked

            currentPos += step;
        }

        return true; // Castling is allowed
    }

    // ** Override hook to Track King Movement**
    protected override void hook()
    {
        List<Vector2> legalMoves = GetLegalMoves(oldPos, newPos);

        foreach (Vector2 legalMove in legalMoves)
        {
            if (newPos == legalMove)
            {
                // **Check if this move is a castling move**
                if (Mathf.Abs(newPos.x - oldPos.x) == 2) // Castling happens when King moves 2 squares
                {
                    HandleCastling(newPos);
                }

                // **Move the King in dictionary**
                pieceSetup.pieceDictionary.Remove(oldPos);
                pieceSetup.pieceDictionary[newPos] = gameObject;
                transform.position = newPos;

                // **If this is the first move, mark king as having moved**
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

    private void HandleCastling(Vector2 kingNewPos)
    {
        Vector2 rookOldPos, rookNewPos;

        if (kingNewPos.x > oldPos.x) // **Kingside castling**
        {
            rookOldPos = new Vector2(oldPos.x + 3, oldPos.y);
            rookNewPos = new Vector2(oldPos.x + 1, oldPos.y);
        }
        else // **Queenside castling**
        {
            rookOldPos = new Vector2(oldPos.x - 4, oldPos.y);
            rookNewPos = new Vector2(oldPos.x - 1, oldPos.y);
        }

        // **Find the Rook in the Dictionary**
        if (pieceSetup.pieceDictionary.ContainsKey(rookOldPos))
        {
            GameObject rook = pieceSetup.pieceDictionary[rookOldPos];
            RookBehavior rookBehavior = rook.GetComponent<RookBehavior>();

            // **Move the Rook**
            pieceSetup.pieceDictionary.Remove(rookOldPos);
            pieceSetup.pieceDictionary[rookNewPos] = rook;
            rook.transform.position = rookNewPos;

            // **Mark the rook as having moved**
            if (rookBehavior != null)
            {
                rookBehavior.SetHasMoved();
            }

            Debug.Log($" Castling complete: Rook moved from {rookOldPos} to {rookNewPos}");
        }
        else
        {
            Debug.LogError($" Castling failed: No rook found at {rookOldPos}");
        }
    }


}
