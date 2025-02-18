using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private bool isWhiteTurn = true;
    private KeyValuePair<Vector2, Vector2> lastMove;
    private Stack<KeyValuePair<Vector2, Vector2>> movesHistory = new Stack<KeyValuePair<Vector2, Vector2>>();
    public Text turnText; // Assign in Unity Inspector

    public KeyValuePair<Vector2, Vector2> GetLastMove()
    {
        return lastMove;
    }
    public void SetLastMove(KeyValuePair<Vector2, Vector2> move)
    {
        lastMove = move;
    }

    private void Awake()
    {
        Debug.Log("TurnManager On");
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateTurnText();
    }

    public bool IsWhiteTurn()
    {
        return isWhiteTurn;
    }

    public void SwitchTurn()
    {
        Debug.Log($"Last move: {lastMove}"); // Remove later
        movesHistory.Push(lastMove);
        isWhiteTurn = !isWhiteTurn;
        UpdateTurnText();
    }

    private void UpdateTurnText()
    {
        if (turnText != null)
        {
            turnText.text = isWhiteTurn ? "White's Turn" : "Black's Turn";
        }
    }
}
