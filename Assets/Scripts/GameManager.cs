using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText; public float surviveTime = 30f; private bool isGameOver = false;

    void Start() { Time.timeScale = 1; }

    void Update() { if (isGameOver) return; surviveTime -= Time.deltaTime; if (timerText != null) { timerText.text = Mathf.CeilToInt(surviveTime).ToString(); } if (surviveTime <= 0) { WinGame(); } }

    void WinGame() { isGameOver = true; surviveTime = 0; if (timerText != null) { timerText.text = "Victory!"; timerText.color = Color.green; } Debug.Log("Victory!"); Time.timeScale = 0; }
}