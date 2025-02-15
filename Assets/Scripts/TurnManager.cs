using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private bool isWhiteTurn = true;
    public Text turnText; // Assign in Unity Inspector

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
