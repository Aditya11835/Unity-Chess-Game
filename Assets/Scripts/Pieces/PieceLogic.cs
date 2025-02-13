using UnityEngine;

public abstract class PieceLogic : MonoBehaviour
{
    public bool isWhite;
    public char pieceType;
    public abstract bool IsValidMove(Vector3 start, Vector3 end, char pieceType);
}
