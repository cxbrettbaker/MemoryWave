using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{

    public GameObject Score;
    public GameObject Accuracy;
    public GameObject NormalHits;
    public GameObject GoodHits;

    // Start is called before the first frame update
    void Start()
    {
        Score.GetComponent<TextMeshProUGUI>().text = "Score:" + ScoreManager.Instance.score.ToString();
        Accuracy.GetComponent<TextMeshProUGUI>().text = "Accuracy: " + ScoreManager.Instance.CalculateAccuracy().ToString("F2") + "%";
        NormalHits.GetComponent<TextMeshProUGUI>().text = "Great Hits: " + ScoreManager.Instance.numGreat.ToString();
        GoodHits.GetComponent<TextMeshProUGUI>().text = "Good Hits: " + ScoreManager.Instance.numGood.ToString();
    }

    public void OnRetryClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnBackClick()
    {
        SceneManager.LoadScene("SongSelect");
    }
}
