using UnityEngine;


public class ReadOnlyAttribute : PropertyAttribute
{
}

public class BaseInputAttribute : PropertyAttribute
{
}

public class PlayableInputAttribute : BaseInputAttribute
{
}

public static class TransformDeepChildExtension
{
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result != null)
            return result;

        foreach (Transform child in parent)
        {
            result = child.FindDeepChild(name);
            if (result != null)
                return result;
        }

        return null;
    }
}