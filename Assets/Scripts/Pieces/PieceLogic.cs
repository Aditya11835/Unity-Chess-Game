using Unity.VisualScripting;
using UnityEngine;

public abstract class PieceLogic : MonoBehaviour
{
    public bool isWhite;
    
    public abstract bool IsValidMove(Vector3 start, Vector3 end);
    public int PositionToBoard(float pos)
    {
        return Mathf.RoundToInt(pos + 3.5f);
    }
    public float BoardToPosition(int board)
    {
        return board - 3.5f;
    }
}
