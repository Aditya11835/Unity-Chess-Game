using UnityEngine;

public class Pawn : PieceLogic
{
    void Start()
    {
        pieceType = 'P'; // Indicating it's a Pawn
    }

    public override bool IsValidMove(Vector3 start, Vector3 end, char pieceType)
    {
        // Convert positions to board coordinates (0 to 7)
        int startX = Mathf.RoundToInt(start.x + 3.5f);
        int startY = Mathf.RoundToInt(start.y + 3.5f);
        int endX = Mathf.RoundToInt(end.x + 3.5f);
        int endY = Mathf.RoundToInt(end.y + 3.5f);

        int direction = isWhite ? 1 : -1; // White pawns move up, black pawns move down

        // Normal forward move (one step)
        if (endX == startX && endY == startY + direction)
        {
            return true;
        }

        // First move (two-step move from starting position)
        if ((isWhite && startY == 1) || (!isWhite && startY == 6))
        {
            if (endX == startX && endY == startY + (2 * direction))
            {
                return true;
            }
        }

        // TODO: Add diagonal capturing logic if implementing full chess logic.

        return false; // If none of the conditions match, move is invalid
    }
}
