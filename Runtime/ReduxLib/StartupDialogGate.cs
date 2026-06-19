using System.Collections;
using UnityEngine;

namespace ReduxLib;

public static class StartupDialogGate
{
    public static bool IsStartupPresentationActive { get; private set; }

    public static bool IsStartupLegalFlowActive { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        IsStartupPresentationActive = false;
        IsStartupLegalFlowActive = false;
    }

    public static bool IsBlockingDialogs => IsStartupPresentationActive || IsStartupLegalFlowActive;

    public static void SetStartupPresentationActive(bool isActive)
    {
        IsStartupPresentationActive = isActive;
    }

    public static void SetStartupLegalFlowActive(bool isActive)
    {
        IsStartupLegalFlowActive = isActive;
    }

    public static IEnumerator WaitUntilReadyForDialogs(int requiredClearFrames = 2)
    {
        int clearFrames = 0;
        while (clearFrames < requiredClearFrames)
        {
            if (IsBlockingDialogs)
            {
                clearFrames = 0;
            }
            else
            {
                clearFrames++;
            }

            yield return null;
        }
    }

    public static IEnumerator WaitUntilStartupPresentationComplete(int requiredClearFrames = 2)
    {
        int clearFrames = 0;
        while (clearFrames < requiredClearFrames)
        {
            if (IsStartupPresentationActive)
            {
                clearFrames = 0;
            }
            else
            {
                clearFrames++;
            }

            yield return null;
        }
    }
}
