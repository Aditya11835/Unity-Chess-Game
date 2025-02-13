using UnityEngine;

public class Pawn : PieceLogic
{
    public override bool IsValidMove(Vector3 start, Vector3 end)
    {
        // Convert positions to board coordinates (0 to 7)->(1 to 8)
        int startX = PositionToBoard(start.x);
        int startY = PositionToBoard(start.y);
        int endX = PositionToBoard(end.x);
        int endY = PositionToBoard(end.y);
        
        

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
