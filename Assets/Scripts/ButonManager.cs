using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Game"); // Ensure the scene name matches exactly
    }

    public void QuitGame()
    {
        Application.Quit(); // Closes the application
    }
}
