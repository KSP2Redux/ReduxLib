using System;

namespace ReduxLib.GameInterfaces;

public interface IFlowAction
{
    public string Name { get; }
    public string Description { get; }
    public void DoAction(Action resolve, Action<string> reject);
}