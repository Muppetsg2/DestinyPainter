public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return from2 + ((to2 - from2) / (to1 - from1)) * (value - from1);
    }

}