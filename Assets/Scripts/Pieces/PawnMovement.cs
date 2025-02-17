using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PawnMovement : PieceBehavior
{
    private bool canPromote;
    private KeyValuePair<Vector2, Vector2> lastMove;
    void checkPromote()
    {
        if (isWhite && transform.position.y == 3.5)
        {
            canPromote = true;
        }
        else if(!isWhite && transform.position.y == -3.5)
        {
            canPromote = true;
        }
    }
    protected override void Start()
    {
        base.Start();
        canPromote = false;
    }
    protected override void OnMouseUp()
    {
        base.OnMouseUp();
    }

    void PromotePawn()
    {
        Vector2 tempPos = gameObject.transform.position;
        Transform pP = pieceSetup.piecesParent;
        string name = gameObject.name.Split('_')[1];
        Destroy(gameObject);
        GameObject piece = isWhite ? Instantiate(pieceSetup.piecePrefabs[8], tempPos, Quaternion.identity, pP) : Instantiate(pieceSetup.piecePrefabs[9], tempPos, Quaternion.identity, pP);
        piece.name = $"Promoted_{piece.name.Split('(')[0]}_{name}";
        piece.transform.localScale = new Vector3(4, 4, 0);
        pieceSetup.pieceDictionary[piece.transform.position] = piece;
    }
    protected override void hook()
    {
        base.hook();
        if(IsLegalMove(oldPos, newPos))
        {
            pieceSetup.pieceDictionary.Remove(oldPos);
            pieceSetup.pieceDictionary[newPos] = gameObject;
            transform.position = newPos;
            if (!canPromote)
            {
                checkPromote();
                if (canPromote)
                {
                    PromotePawn();
                }
            }

            turnFinished = true;
        }
        else
        {
            transform.position = oldPos;
            turnFinished = false;
        }
    }

    protected override bool IsCapture(Vector2 oldPos, Vector2 newPos)
    {
        Vector2 moveDirection = isWhite ? Vector2.up : Vector2.down;
        if (Mathf.Abs(newPos.x - oldPos.x) == 1 && newPos.y - oldPos.y == moveDirection.y)
        {
            if (pieceSetup.pieceDictionary.ContainsKey(newPos))
            {
                GameObject targetPiece = pieceSetup.pieceDictionary[newPos];
                bool isOpponent = (targetPiece.GetComponent<PawnMovement>().isWhite != isWhite);
                if (isOpponent)
                {
                    pieceSetup.pieceDictionary.Remove(newPos);
                    Destroy(targetPiece);
                }
                return isOpponent;
            }

            Vector2 enPassantTargetLocation = new Vector2(newPos.x, oldPos.y);
            KeyValuePair<Vector2, Vector2> lastMove = TurnManager.Instance.GetLastMove();
            if (pieceSetup.pieceDictionary.ContainsKey(enPassantTargetLocation))
            {
                GameObject enPassantTarget = pieceSetup.pieceDictionary[enPassantTargetLocation];
                if(enPassantTarget.GetComponent<PawnMovement>().isWhite != isWhite
                    && lastMove.Key == enPassantTargetLocation - 2*moveDirection && lastMove.Value == enPassantTargetLocation)
                {
                    pieceSetup.pieceDictionary.Remove(enPassantTargetLocation);
                    Destroy(enPassantTarget);
                    return true;
                }

            }
        }
        return false;
    }

    protected override bool IsLegalMove(Vector2 oldPos, Vector2 newPos)
    {
        if (pieceSetup.pieceDictionary == null) return false; // Ensure dictionary exists

        Vector2 moveDirection = isWhite ? Vector2.up : Vector2.down;
        Vector2 startPos = isWhite ? new Vector2(oldPos.x, -2.5f) : new Vector2(oldPos.x, 2.5f);
        bool canDoubleForward = (oldPos == startPos);

        if(newPos == oldPos + moveDirection && !pieceSetup.pieceDictionary.ContainsKey(newPos))
        {
            return true; //newPos is 1 ahead and it is unoccupied
        }

        if(canDoubleForward && newPos == oldPos + moveDirection*2 && !pieceSetup.pieceDictionary.ContainsKey(oldPos + moveDirection) && !pieceSetup.pieceDictionary.ContainsKey(newPos))
        {
            return true;
        }

        if (IsCapture(oldPos, newPos))
        {
            return true;
        }

        return false;
    }
}
