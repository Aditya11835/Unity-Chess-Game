using UnityEngine;

public abstract class PieceLogic : MonoBehaviour
{
    public bool isWhite;
    public abstract bool IsValidMove(Vector3 start, Vector3 end);
}
