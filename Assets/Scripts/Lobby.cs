using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{ 
    //****************** UI event handlers *********************//
    /// <summary>
    /// Practice button was clicked.
    /// </summary>
    public void OnPracticeClicked()
    {
        Debug.Log("OnPracticeClicked");
        SceneManager.LoadScene("GameScene");
    }
}
