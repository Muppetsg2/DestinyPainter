using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Portal portalA;
    public Portal portalB;

    private GameObject clone;
    private Vector3 lastPlayerPos = Vector3.zero;
    private GameObject player;

    private bool teleported = true;

    private void Update()
    {
        if (teleported) return;

        Vector3 posDiff = player.transform.position - lastPlayerPos;
        clone.transform.position = posDiff + clone.transform.position;

        lastPlayerPos = player.transform.position;
    }

    public void TeleportToOther(Portal current, GameObject objectToTeleport)
    {
        teleported = false;

        Portal other;
        if (current == portalA) other = portalB;
        else other = portalA;

        clone = VisualyCloneGameObject(objectToTeleport);

        lastPlayerPos = objectToTeleport.transform.position;
        Vector3 posDiff = objectToTeleport.transform.position - current.transform.position;
        clone.transform.position = posDiff + other.transform.position;

        Vector3 rotDiff = objectToTeleport.transform.rotation.eulerAngles - current.transform.rotation.eulerAngles;
        clone.transform.rotation = Quaternion.Euler(other.transform.rotation.eulerAngles + rotDiff);

        player = objectToTeleport;
    }

    public void TeleportEnd(Portal current, GameObject teleportedObject)
    {
        teleported = true;
    }

    private GameObject VisualyCloneGameObject(GameObject gameObject)
    {
        GameObject clonned = new(gameObject.name + " (Visual Clone)");
        if (gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
        {
            clonned.CopyComponent<SpriteRenderer>(renderer);
        }

        Transform cloneTransform = clonned.transform;
        Transform objTransform = gameObject.transform;

        cloneTransform.rotation = objTransform.rotation;
        cloneTransform.localScale = objTransform.localScale;

        for (int i = 0; i < objTransform.childCount; ++i)
        {
            VisualyCloneGameObject(objTransform.GetChild(i).gameObject).transform.parent = cloneTransform;
        }

        return clonned;
    }
}

static class ComponentExtension
{
    public static T CopyComponent<T>(this GameObject dest, T original) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = dest.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(copy, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(copy, prop.GetValue(original, null), null);
        }
        return copy as T;
    }
}
