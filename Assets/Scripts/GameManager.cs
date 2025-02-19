using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    private PieceSetup pieceSetup;
    private TurnManager turnManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        pieceSetup = FindAnyObjectByType<PieceSetup>();
        turnManager = TurnManager.Instance;
    }

    public void CheckGameState()
    {
        bool isWhiteTurn = turnManager.IsWhiteTurn();
        Debug.Log($"[GameState] Checking game state for {(isWhiteTurn ? "White" : "Black")}");

        GameObject king = FindKing(isWhiteTurn);

        if (king == null)
        {
            Debug.LogError("[GameState] ERROR: King not found!");
            return;
        }

        Debug.Log($"[GameState] King found at {king.transform.position}");

        // **Check Conditions**
        bool isInCheck = IsKingInCheck(king);
        bool hasLegalMoves = HasLegalMoves(isWhiteTurn);

        if (isInCheck && !hasLegalMoves)
        {
            Debug.Log($"[GameState] Checkmate! {(isWhiteTurn ? "Black" : "White")} wins!");
        }
        else if (!isInCheck && !hasLegalMoves)
        {
            Debug.Log("[GameState] Stalemate! The game is a draw.");
        }
        else if (isInCheck)
        {
            Debug.Log($"[GameState] {(isWhiteTurn ? "White" : "Black")} is in Check!");
        }
        else
        {
            Debug.Log($"[GameState] {(isWhiteTurn ? "White" : "Black")} has legal moves.");
        }
    }

    private GameObject FindKing(bool isWhite)
    {
        foreach (var entry in pieceSetup.pieceDictionary)
        {
            PieceBehavior piece = entry.Value.GetComponent<PieceBehavior>();
            if (piece is KingBehavior && piece.isWhite == isWhite)
            {
                return entry.Value;
            }
        }
        return null;
    }

    private bool IsKingInCheck(GameObject king)
    {
        Vector2 kingPos = king.transform.position;
        Debug.Log($"[Check] Checking if king at {kingPos} is in check.");

        foreach (var entry in pieceSetup.pieceDictionary)
        {
            PieceBehavior piece = entry.Value.GetComponent<PieceBehavior>();
            if (piece != null && piece.isWhite != king.GetComponent<PieceBehavior>().isWhite)
            {
                List<Vector2> opponentMoves = piece.GetLegalMoves(piece.transform.position, kingPos, true); // **Pass true to get only capture moves**

                if (opponentMoves.Contains(kingPos))
                {
                    Debug.Log($"[Check] King at {kingPos} is in check from {piece.gameObject.name} at {piece.transform.position}");
                    return true;
                }
            }
        }
        Debug.Log("[Check] King is NOT in check.");
        return false;
    }


    private bool HasLegalMoves(bool isWhite)
    {
        Debug.Log($"[LegalMoves] Checking if {(isWhite ? "White" : "Black")} has legal moves.");

        foreach (var entry in pieceSetup.pieceDictionary)
        {
            PieceBehavior piece = entry.Value.GetComponent<PieceBehavior>();
            if (piece != null && piece.isWhite == isWhite)
            {
                List<Vector2> legalMoves = piece.GetLegalMoves(piece.transform.position, piece.transform.position, true);

                foreach (Vector2 move in legalMoves)
                {
                    if (SimulateMoveAndCheckSafety(piece, move))
                    {
                        Debug.Log($"[LegalMoves] Piece {piece.gameObject.name} at {piece.transform.position} has a legal move to {move}");
                        return true;
                    }
                }
            }
        }

        Debug.Log($"[LegalMoves] No legal moves for {(isWhite ? "White" : "Black")}.");
        return false;
    }

    private bool SimulateMoveAndCheckSafety(PieceBehavior piece, Vector2 targetPos)
    {
        Vector2 originalPos = piece.transform.position;
        Debug.Log($"[Simulation] Simulating move: {piece.gameObject.name} from {originalPos} to {targetPos}");

        // **Temporarily move the piece**
        GameObject capturedPiece = null;
        if (pieceSetup.pieceDictionary.ContainsKey(targetPos))
        {
            capturedPiece = pieceSetup.pieceDictionary[targetPos];
            pieceSetup.pieceDictionary.Remove(targetPos);
            Debug.Log($"[Simulation] Temporarily capturing {capturedPiece.name} at {targetPos}");
        }

        pieceSetup.pieceDictionary.Remove(originalPos);
        pieceSetup.pieceDictionary[targetPos] = piece.gameObject;
        piece.transform.position = targetPos;

        // **Check if king is still in check**
        GameObject king = FindKing(piece.isWhite);
        bool isSafe = !IsKingInCheck(king);
        Debug.Log($"[Simulation] Move is {(isSafe ? "SAFE" : "NOT SAFE")} for {piece.gameObject.name}");

        // **Revert move**
        pieceSetup.pieceDictionary.Remove(targetPos);
        pieceSetup.pieceDictionary[originalPos] = piece.gameObject;
        piece.transform.position = originalPos;

        if (capturedPiece != null)
        {
            pieceSetup.pieceDictionary[targetPos] = capturedPiece;
            Debug.Log($"[Simulation] Restoring {capturedPiece.name} at {targetPos}");
        }

        return isSafe;
    }
}
