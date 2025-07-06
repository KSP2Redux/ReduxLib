using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReduxLib.Input;

public static class InputHelper
{
    public static bool IsRebindingHappening { get; private set; }
    public static KeyBindRebindingOperation? ActiveKeyBindRebindingOperation;
    public static KeyboardShortcutRebindingOperation? ActiveKeyboardShortcutRebindingOperation;
    
    public class KeyBindRebindingOperation
    {
        public event Action OnCancel;
        public event Action<KeyBind> OnSuccess;
        public bool IsFinished;
        public bool IsStarted;

        public void Start()
        {
            if (IsFinished || IsStarted) return;
            if (IsRebindingHappening)
            {
                ActiveKeyBindRebindingOperation?.Cancel();
                ActiveKeyboardShortcutRebindingOperation?.Cancel();
            }
            ActiveKeyBindRebindingOperation = this;
            IsStarted = true;
            ReduxLib.Instance.StartCoroutine(Execute());
        }
        
        public void Cancel()
        {
            if (!IsStarted) return;
            IsRebindingHappening = false;
            ActiveKeyBindRebindingOperation = null;
            IsFinished = true;
            OnCancel?.Invoke();
        }

        private void Finish(KeyBind binding)
        {
            IsFinished = true;
            IsRebindingHappening = false;
            ActiveKeyBindRebindingOperation = null;
            OnSuccess?.Invoke(binding);
        }

        private IEnumerator Execute()
        {
            while (!IsFinished)
            {
                yield return null;
                if (GetKeyDown() is not { } keyCode) continue;
                if (keyCode == KeyCode.Escape)
                {
                    Cancel();
                }
                else
                {
                    Finish(new KeyBind(keyCode));
                }
            }
        }
    }

    public class KeyboardShortcutRebindingOperation
    {
        public event Action OnCancel;
        public event Action<KeyboardShortcut> OnSuccess;
        public bool IsFinished;
        public bool IsStarted;
        public void Start()
        {
            if (IsFinished || IsStarted) return;
            if (IsRebindingHappening)
            {
                ActiveKeyBindRebindingOperation?.Cancel();
                ActiveKeyboardShortcutRebindingOperation?.Cancel();
            }
            ActiveKeyboardShortcutRebindingOperation = this;
            IsStarted = true;
            ReduxLib.Instance.StartCoroutine(Execute());
        }
        
        public void Cancel()
        {
            if (!IsStarted) return;
            IsRebindingHappening = false;
            ActiveKeyboardShortcutRebindingOperation = null;
            IsFinished = true;
            OnCancel?.Invoke();
        }
        
        private void Finish(KeyboardShortcut binding)
        {
            IsFinished = true;
            IsRebindingHappening = false;
            ActiveKeyBindRebindingOperation = null;
            OnSuccess?.Invoke(binding);
        }
        
        private IEnumerator Execute()
        {
            while (!IsFinished)
            {
                yield return null;
                if (GetKeyDown() is not { } keyCode) continue;
                if (keyCode == KeyCode.Escape)
                {
                    Cancel();
                }
                else
                {
                    Finish(new (keyCode, GetModifiers()));
                }
            }
        }

        private static KeyCode[] Modifiers = {
            KeyCode.LeftControl,
            KeyCode.RightControl,
            KeyCode.LeftShift,
            KeyCode.RightShift,
            KeyCode.LeftAlt,
            KeyCode.RightAlt,
        };
        
        private KeyCode[] GetModifiers()
        {
            List<KeyCode> activeModifiers = new();
            foreach (var modifier in Modifiers)
            {
                if (UnityEngine.Input.GetKey(modifier))
                {
                    activeModifiers.Add(modifier);
                }
            }
            return activeModifiers.ToArray();
        }
    }

    public static KeyBindRebindingOperation RebindKeyBind()
    {
        return new KeyBindRebindingOperation();
    }

    public static KeyboardShortcutRebindingOperation RebindKeyboardShortcut()
    {
        return new KeyboardShortcutRebindingOperation();
    }


    public static KeyCode? GetKeyDown()
    {
        foreach (var value in Enum.GetValues(typeof(KeyCode)))
        {
            if (UnityEngine.Input.GetKeyDown((KeyCode)value))
            {
                return (KeyCode)value;
            }
        }
        return null;
    }
}
