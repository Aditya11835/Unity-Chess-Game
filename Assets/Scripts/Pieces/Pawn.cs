using UnityEngine;

public class Pawn : PieceLogic
{

    public override bool IsValidMove(Vector3 start, Vector3 end)
    {
        // Convert positions to board coordinates (0 to 7)->(1 to 8)
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
        if ((isWhite && startY == 1) || (!isWhite /*isBlack*/ && startY == 6))
        {
            if (endX == startX && endY == startY + (2 * direction))
            {
                return true;
            }
        }

        // To-Do: Add diagonal capturing logic

        return false; // If none of the conditions match, move is invalid
    }
}
