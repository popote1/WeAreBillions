using System;
using Unity.VisualScripting;
using UnityEngine;

public static class InGameStatic
{
    public static Vector3 CameraMoveVector = Vector3.zero;
    public static bool BlockCameraMovement = false;
    public static bool AllowCameraHeight = true;

    public static event EventHandler OnPressEscape;

    
    public static void PressEscape(object sender) => OnPressEscape?.Invoke(sender, EventArgs.Empty);
}
