using UnityEngine;

public class KnightBehavior : PieceBehavior
{
    protected override void OnMouseUp()
    {
        base.OnMouseUp();  
    }
    protected override void hook()
    {
        base.hook();
        if (IsLegalMove(oldPos, newPos))
        {
            pieceSetup.pieceDictionary.Remove(oldPos);
            pieceSetup.pieceDictionary[newPos] = gameObject;
            transform.position = newPos;

            turnFinished = true;
        }
        else
        {
            transform.position = oldPos;
            turnFinished = false;
        }
    }

    protected override bool IsLegalMove(Vector2 oldPos, Vector2 newPos)
    {
        if (pieceSetup.pieceDictionary == null) return false; // Ensure dictionary exists

        Vector2 displacementVector = newPos - oldPos;

        if (Mathf.Abs(displacementVector.x * displacementVector.y) == 2)
        {
            if (pieceSetup.pieceDictionary.ContainsKey(newPos))
            {
                if (IsCapture(oldPos, newPos)){
                    return true;
                }
            }
            else {
                return true;
            }
        }
        return false;
    }

    protected override bool IsCapture(Vector2 oldPos, Vector2 newPos)
    {
        GameObject targetPiece = pieceSetup.pieceDictionary[newPos];
        if (targetPiece.GetComponent<PieceBehavior>() == null)
        {
            Debug.Log("Capturing logic of this piece is not yet written."); return false;
        }
        bool isOpponent = (targetPiece.GetComponent<PieceBehavior>().isWhite != isWhite);

        if (isOpponent)
        {
            pieceSetup.pieceDictionary.Remove(newPos);
            Destroy(targetPiece);
        }
        return isOpponent;
    }
}
