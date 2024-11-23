using System;

namespace ReduxLib.GameInterfaces;

public abstract class BaseFlowAction : IFlowAction
{
    protected BaseFlowAction(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }
    public string Description { get; }
    public abstract void DoAction(Action resolve, Action<string> reject);
}