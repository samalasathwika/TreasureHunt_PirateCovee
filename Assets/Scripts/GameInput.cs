using UnityEngine;

// Keyboard input that works with the legacy Input Manager, the new Input System, or both.
public static class GameInput
{
#if ENABLE_INPUT_SYSTEM
    static UnityEngine.InputSystem.Keyboard K { get { return UnityEngine.InputSystem.Keyboard.current; } }
#endif

    public static bool LeftPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return K != null && (K.aKey.wasPressedThisFrame || K.leftArrowKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
#endif
    }
    public static bool RightPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return K != null && (K.dKey.wasPressedThisFrame || K.rightArrowKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
#endif
    }
    public static bool JumpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return K != null && (K.spaceKey.wasPressedThisFrame || K.upArrowKey.wasPressedThisFrame || K.wKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
#endif
    }
    public static bool PausePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return K != null && (K.escapeKey.wasPressedThisFrame || K.pKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
#endif
    }
    public static bool RestartPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return K != null && (K.rKey.wasPressedThisFrame || K.enterKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return);
#endif
    }
}
