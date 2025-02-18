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
        else if (!isWhite && transform.position.y == -3.5)
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
        List<Vector2> legalMoves = GetLegalMoves(oldPos, newPos);

        foreach (Vector2 legalMove in legalMoves)
        {
            if (newPos == legalMove)
            {
                // Check if the move is a capture
                if (IsCapture(oldPos, newPos))
                {
                    // If the destination square is occupied, do a normal capture.
                    if (pieceSetup.pieceDictionary.ContainsKey(newPos))
                    {
                        GameObject targetPiece = pieceSetup.pieceDictionary[newPos];
                        pieceSetup.pieceDictionary.Remove(newPos);
                        Destroy(targetPiece);
                    }
                    // Otherwise, if newPos is empty, check for en passant capture.
                    else
                    {
                        Vector2 enPassantTargetLocation = new Vector2(newPos.x, oldPos.y);
                        if (pieceSetup.pieceDictionary.ContainsKey(enPassantTargetLocation))
                        {
                            GameObject enPassantTarget = pieceSetup.pieceDictionary[enPassantTargetLocation];
                            pieceSetup.pieceDictionary.Remove(enPassantTargetLocation); // Remove the en passant target.
                            Destroy(enPassantTarget);
                        }
                    }
                }

                // Move the pawn in dictionary and update its position.
                pieceSetup.pieceDictionary.Remove(oldPos);
                pieceSetup.pieceDictionary[newPos] = gameObject;
                transform.position = newPos;
                checkPromote();
                if (canPromote)
                {
                    PromotePawn();
                }

                turnFinished = true;
                return;
            }
        }

        // Invalid move: revert position.
        transform.position = oldPos;
        turnFinished = false;
    }





    protected override bool IsCapture(Vector2 oldPos, Vector2 newPos)
    {
        Vector2 moveDirection = isWhite ? Vector2.up : Vector2.down;

        // **Normal diagonal capture**
        if (Mathf.Abs(newPos.x - oldPos.x) == 1 && newPos.y - oldPos.y == moveDirection.y)
        {
            if (pieceSetup.pieceDictionary.ContainsKey(newPos))
            {
                GameObject targetPiece = pieceSetup.pieceDictionary[newPos];
                if (targetPiece.GetComponent<PieceBehavior>() == null)
                {
                    Debug.Log("Capturing logic of this piece is not yet written.");
                    return false;
                }

                return targetPiece.GetComponent<PieceBehavior>().isWhite != isWhite;
            }

            // **En Passant Capture Check**
            Vector2 enPassantTargetLocation = new Vector2(newPos.x, oldPos.y);
            KeyValuePair<Vector2, Vector2> lastMove = TurnManager.Instance.GetLastMove();

            if (IsValidEnPassantMove(oldPos, enPassantTargetLocation, lastMove))
            {
                return true; //  En Passant capture is valid
            }
        }

        return false;
    }



    protected override List<Vector2> GetLegalMoves(Vector2 oldPos, Vector2 newPos)
    {
        List<Vector2> legalMoves = new List<Vector2>();

        if (pieceSetup.pieceDictionary == null) return legalMoves;

        Vector2 moveDirection = isWhite ? Vector2.up : Vector2.down;
        Vector2 startPos = isWhite ? new Vector2(oldPos.x, -2.5f) : new Vector2(oldPos.x, 2.5f);
        bool canDoubleForward = (oldPos == startPos);

        // **Normal Forward Moves**
        Vector2 oneStep = oldPos + moveDirection;
        if (!pieceSetup.pieceDictionary.ContainsKey(oneStep))
        {
            legalMoves.Add(oneStep);
        }

        // **Double Forward Move (if first move)**
        Vector2 twoSteps = oldPos + moveDirection * 2;
        if (canDoubleForward && !pieceSetup.pieceDictionary.ContainsKey(oneStep) && !pieceSetup.pieceDictionary.ContainsKey(twoSteps))
        {
            legalMoves.Add(twoSteps);
        }

        // **Capture Moves (Diagonal)**
        Vector2 leftCapture = oldPos + new Vector2(-1, moveDirection.y);
        Vector2 rightCapture = oldPos + new Vector2(1, moveDirection.y);

        if (IsCapture(oldPos, leftCapture))
        {
            legalMoves.Add(leftCapture);
        }

        if (IsCapture(oldPos, rightCapture))
        {
            legalMoves.Add(rightCapture);
        }

        // **Fix En Passant Moves**
        KeyValuePair<Vector2, Vector2> lastMove = TurnManager.Instance.GetLastMove();
        Vector2 leftEnPassantTarget = new Vector2(oldPos.x - 1, oldPos.y);
        Vector2 rightEnPassantTarget = new Vector2(oldPos.x + 1, oldPos.y);

        // **The squares the pawn will land if it captures En Passant**
        Vector2 leftEnPassantCapture = new Vector2(leftEnPassantTarget.x, oldPos.y + moveDirection.y);
        Vector2 rightEnPassantCapture = new Vector2(rightEnPassantTarget.x, oldPos.y + moveDirection.y);

        if (IsValidEnPassantMove(oldPos, leftEnPassantTarget, lastMove))
        {
            legalMoves.Add(leftEnPassantCapture);
        }

        if (IsValidEnPassantMove(oldPos, rightEnPassantTarget, lastMove))
        {
            legalMoves.Add(rightEnPassantCapture);
        }

        return legalMoves;
    }

    private bool IsValidEnPassantMove(Vector2 oldPos, Vector2 enPassantTargetLocation, KeyValuePair<Vector2, Vector2> lastMove)
    {
        if (!pieceSetup.pieceDictionary.ContainsKey(enPassantTargetLocation))
        {
            return false; // No piece at the En Passant capture position
        }

        GameObject enPassantTarget = pieceSetup.pieceDictionary[enPassantTargetLocation];

        if (enPassantTarget == null || enPassantTarget.GetComponent<PawnMovement>() == null)
        {
            return false; // Not a pawn
        }

        bool isOpponentPawn = enPassantTarget.GetComponent<PawnMovement>().isWhite != isWhite;
        Vector2 expectedStart = lastMove.Key;
        Vector2 expectedEnd = lastMove.Value;
        Vector2 moveDirection = isWhite ? Vector2.down : Vector2.up; // Opponent's movement

        bool movedTwoSteps = Mathf.Approximately(expectedStart.y + (moveDirection.y * 2), expectedEnd.y) &&
                             Mathf.Approximately(expectedStart.x, expectedEnd.x) &&
                             Mathf.Approximately(expectedEnd.x, enPassantTargetLocation.x);

        return isOpponentPawn && movedTwoSteps;
    }

}