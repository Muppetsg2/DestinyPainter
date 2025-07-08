using SaintsField;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public PortalManager parent;
    [Tag]
    public string playerTag;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;

        parent.TeleportToOther(this, collision.gameObject);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;
        parent.TeleportEnd(this, collision.gameObject);
    }
}
