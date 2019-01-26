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
        Score.GetComponent<TextMeshProUGUI>().text = "Score:" + GameManager.instance.score.ToString();
        Accuracy.GetComponent<TextMeshProUGUI>().text = "Accuracy" + 
            ((GameManager.instance.numNormalHit + GameManager.instance.numGoodHit) / GameManager.instance.hitObjectsList.Count).ToString();
        NormalHits.GetComponent<TextMeshProUGUI>().text = "Normal Hits: " + GameManager.instance.numNormalHit.ToString();
        GoodHits.GetComponent<TextMeshProUGUI>().text = "Good Hits: " + GameManager.instance.numGoodHit.ToString();
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
