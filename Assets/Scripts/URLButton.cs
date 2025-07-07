using UnityEngine;

public class URLButton : MonoBehaviour
{
    [SerializeField] private string url;

    public void Open()
    {
        Application.OpenURL(url);
    }
}
