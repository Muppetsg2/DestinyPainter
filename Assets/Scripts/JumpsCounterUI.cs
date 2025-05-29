using TMPro;
using UnityEngine;

public class JumpsCounterUI : MonoBehaviour
{
    public PlayerController player;
    public TextMeshProUGUI text;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        text.text = player.planetJumpsCounter.ToString();
    }
}
