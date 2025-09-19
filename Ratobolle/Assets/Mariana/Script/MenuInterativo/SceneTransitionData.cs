using UnityEngine;

public static class SceneTransitionData
{
    public static bool HasData { get; private set; } = false;

    public static Vector3 PlayerPosition { get; private set; }
    public static Quaternion PlayerRotation { get; private set; }
    public static Vector3 CameraPosition { get; private set; }
    public static Quaternion CameraRotation { get; private set; }

    public static void SetData(Transform player, Transform camera)
    {
        PlayerPosition = player.position;
        PlayerRotation = player.rotation;
        CameraPosition = camera.position;
        CameraRotation = camera.rotation;
        HasData = true;
    }

    public static void ClearData()
    {
        HasData = false;
    }
}
