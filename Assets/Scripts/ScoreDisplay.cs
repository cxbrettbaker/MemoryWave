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
        Score.GetComponent<TextMeshProUGUI>().text = "Score:" + GameManager.Instance.score.ToString();
        Accuracy.GetComponent<TextMeshProUGUI>().text = "Accuracy: WIP";
        NormalHits.GetComponent<TextMeshProUGUI>().text = "Normal Hits: " + GameManager.Instance.numNormalHit.ToString();
        GoodHits.GetComponent<TextMeshProUGUI>().text = "Good Hits: " + GameManager.Instance.numGoodHit.ToString();
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
