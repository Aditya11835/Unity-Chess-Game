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
        Vector2 tempPos = transform.position; // Save pawn position before destroying it
        Transform pP = pieceSetup.piecesParent;

        string name = gameObject.name.Split('_')[1];

        //  Remove pawn from piece dictionary before destroying it
        pieceSetup.pieceDictionary.Remove(tempPos);

        //  Destroy the pawn AFTER removing it from dictionary
        Destroy(gameObject);

        //  Correctly instantiate a promoted Queen
        GameObject promotedPiece = isWhite
            ? Instantiate(pieceSetup.piecePrefabs[8], tempPos, Quaternion.identity, pP)
            : Instantiate(pieceSetup.piecePrefabs[9], tempPos, Quaternion.identity, pP);

        promotedPiece.name = $"Promoted_{promotedPiece.name.Split('(')[0]}_{name}";
        promotedPiece.transform.localScale = new Vector3(4, 4, 0);

        //  Update piece dictionary with new promoted piece
        pieceSetup.pieceDictionary[tempPos] = promotedPiece;
    }

    protected override void hook()
    {
        base.hook();

        if (IsLegalMove(oldPos, newPos))
        {
            pieceSetup.pieceDictionary.Remove(oldPos);
            pieceSetup.pieceDictionary[newPos] = gameObject;
            transform.position = newPos;

            //  Ensure promotion is checked after moving
            checkPromote();
            if (canPromote)
            {
                PromotePawn();
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
                if(targetPiece.GetComponent<PieceBehavior>() == null)
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

            Vector2 enPassantTargetLocation = new Vector2(newPos.x, oldPos.y);
            KeyValuePair<Vector2, Vector2> lastMove = TurnManager.Instance.GetLastMove();

            Debug.Log($" En Passant Target Location: {enPassantTargetLocation}");
            Debug.Log($" Last Move: {lastMove.Key} -> {lastMove.Value}");

            if (!pieceSetup.pieceDictionary.ContainsKey(enPassantTargetLocation))
            {
                Debug.Log(" No piece found at enPassantTargetLocation in dictionary.");
                return false;
            }

            GameObject enPassantTarget = pieceSetup.pieceDictionary[enPassantTargetLocation];
            if (enPassantTarget == null || enPassantTarget.GetComponent<PawnMovement>() == null)
            {
                Debug.Log(" En Passant target is missing or not a pawn.");
                return false;
            }

            bool isOpponentPawn = enPassantTarget.GetComponent<PawnMovement>().isWhite != isWhite;

            //  Expected Start and End Positions for En Passant
            Vector2 expectedStart = lastMove.Key; //  Use the actual starting position from lastMove
            Vector2 expectedEnd = enPassantTargetLocation;

            Debug.Log($" Expected Start: {expectedStart}, Expected End: {expectedEnd}");

            //  Fix: Use Mathf.Approximately() for floating-point comparisons
            bool isCorrectStart = Mathf.Approximately(lastMove.Key.x, expectedStart.x) && Mathf.Approximately(lastMove.Key.y, expectedStart.y);
            bool isCorrectEnd = Mathf.Approximately(lastMove.Value.x, expectedEnd.x) && Mathf.Approximately(lastMove.Value.y, expectedEnd.y);

            if (isOpponentPawn && isCorrectStart && isCorrectEnd)
            {
                Debug.Log(" En Passant Capture Successful!");
                pieceSetup.pieceDictionary.Remove(enPassantTargetLocation);
                Destroy(enPassantTarget);
                return true;
            }
            else
            {
                Debug.Log(" En Passant Conditions Not Met!");
                return false;
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
