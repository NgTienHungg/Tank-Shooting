using UnityEngine;

public static class DebugColor
{
    public const string Red = "red";
    public const string Green = "green";
    public const string Blue = "blue";
    public const string Yellow = "yellow";
    public const string Orange = "orange";
    public const string Lime = "lime";
    public const string Cyan = "cyan";
    public const string Magenta = "magenta";
    public const string White = "white";
    public const string Black = "black";

    public static void Log(string message, string color)
    {
        Debug.Log($"<color={color}>{message}</color>");
    }

    public static void LogWarning(string message, string color)
    {
        Debug.LogWarning($"<color={color}>{message}</color>");
    }

    public static void LogError(string message, string color)
    {
        Debug.LogError($"<color={color}>{message}</color>");
    }
}