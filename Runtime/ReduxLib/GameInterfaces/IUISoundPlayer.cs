using UnityEngine.UIElements;

namespace ReduxLib.GameInterfaces;

public interface IUISoundPlayer
{
    public static IUISoundPlayer Instance;

    public void PostAKEventWithPositionalRTPC(string eventToPost, VisualElement target);
}
