using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class GameManager : MonoBehaviour
{
    // Call this when the player dies
    public void GameOver()
    {
        // Load the Game Over scene (change "GameOverScene" to your actual scene name)
        SceneManager.LoadScene("GameOverScene");
    }

    // Call this when the enemy dies (after being hit 15 times)
    public void Win()
    {
        // Load the Win scene (change "WinScene" to your actual scene name)
        SceneManager.LoadScene("WinScene");
    }
}
