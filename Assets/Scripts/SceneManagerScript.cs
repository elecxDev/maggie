using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    // Function to load the beginning scene (Scene 4 - index 3) when the game first starts
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) // If the current scene is the default one (first loaded)
        {
            SceneManager.LoadScene(3); // Load the 4th scene (index 3)
        }
    }

    // Function to switch to the game scene (Scene 0)
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    // Function to switch to the game over scene (Scene 1)
    public void GameOver()
    {
        SceneManager.LoadScene(0);
    }

    // Function to switch to the win scene (Scene 2)
    public void WinGame()
    {
        SceneManager.LoadScene(2);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(3);
    }
    // Function to quit the game
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
