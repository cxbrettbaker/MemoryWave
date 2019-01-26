using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelect : MonoBehaviour
{
    public void BackButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
