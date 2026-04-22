using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Michsky.UI.Dark
{
    public class PressKeyEvent : MonoBehaviour
    {
        // Settings
        public InputAction hotkey;

        // Events
        public UnityEvent onPressEvent;
        bool firstFrame = true;

        void Start()
        {
            hotkey.Enable();
            hotkey.Reset();
        }

        void Update()
        {
            if (firstFrame)
            {
                firstFrame = false;
                return;
            }

            if (hotkey.triggered)
                onPressEvent.Invoke();
        }
    }
}