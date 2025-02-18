using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class KnightBehavior : PieceBehavior
{
    protected override List<Vector2> GetLegalMoves(Vector2 oldPos, Vector2 newPos)
    {
        List<Vector2> legalMoves = new List<Vector2>();

        if (pieceSetup.pieceDictionary == null) return legalMoves; // Ensure dictionary exists

        // Define all possible knight move offsets (L-shape moves)
        Vector2[] moveOffsets = new Vector2[]
        {
        new Vector2( 1,  2), new Vector2( 1, -2),
        new Vector2(-1,  2), new Vector2(-1, -2),
        new Vector2( 2,  1), new Vector2( 2, -1),
        new Vector2(-2,  1), new Vector2(-2, -1)
        };

        // Iterate through all possible knight moves
        foreach (Vector2 offset in moveOffsets)
        {
            Vector2 potentialMove = oldPos + offset;

            // Ensure the new position is within board boundaries
            if (IsWithinBoard(potentialMove))
            {
                // If the square is empty or it's an opponent's piece, add it as a valid move
                if (!pieceSetup.pieceDictionary.ContainsKey(potentialMove) || IsCapture(oldPos, potentialMove))
                {
                    legalMoves.Add(potentialMove);
                }
            }
        }

        return legalMoves;
    }



    protected override bool IsCapture(Vector2 oldPos, Vector2 newPos)
    {
        // Ensure the target position exists in the dictionary
        if (!pieceSetup.pieceDictionary.ContainsKey(newPos))
        {
            return false; // No piece at the target position
        }

        GameObject targetPiece = pieceSetup.pieceDictionary[newPos];

        // Ensure the target has a PieceBehavior component
        PieceBehavior targetPieceBehavior = targetPiece.GetComponent<PieceBehavior>();
        if (targetPieceBehavior == null)
        {
            Debug.Log("Capturing logic of this piece is not yet written.");
            return false;
        }

        // Capture is valid only if the target is an opponent's piece
        return targetPieceBehavior.isWhite != isWhite;
    }
}