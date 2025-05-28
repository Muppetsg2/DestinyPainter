using TMPro;
using UnityEngine;

public class JumpsCounterUI : MonoBehaviour
{
    public PlayerController player;
    public TextMeshProUGUI text;

    void Update()
    {
        text.text = player.planetJumpsCounter.ToString();
    }
}
