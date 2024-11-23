using System;
using System.Collections.Generic;

namespace ReduxLib.GameInterfaces;

public interface IAssetProvider
{
    public static IAssetProvider Instance;
    
    public bool DoesLabelExist(string label);


    public void LoadByLabel<T>(string label, Action<T> assetLoadCallback, Action<IList<T>>? resultCallback = null)
        where T : UnityEngine.Object;
}