using JetBrains.Annotations;

namespace ReduxLib.Engine;

/// <summary>
/// Contains constants for Unity layers of KSP2.
/// </summary>
[PublicAPI]
public static class Layers
{
    public const int LayerDefault = 0;
    public const int LayerTransparentFx = 1;
    public const int LayerIgnoreRaycast = 2;
    public const int LayerRenderPostProcessing = 3;
    public const int LayerWater = 4;
    public const int LayerUi = 5;
    public const int LayerUiResearchDevelopment = 6;
    public const int LayerPhysxWheel = 7;
    public const int LayerOabScenery = 8;
    public const int LayerOabParts = 9;
    public const int LayerScaledScenery = 10;
    public const int LayerPhysxWheelIgnore = 11;
    public const int LayerRenderDebugVectors = 12;
    public const int LayerKscBuildings = 13;
    public const int LayerLocalTerrain = 14;
    public const int LayerLocalScenery = 15;
    public const int LayerInternalScenery = 16;
    public const int LayerLocalCharacters = 17;
    public const int LayerRenderSkybox = 18;
    public const int LayerLocalParts = 19;
    public const int LayerLocalPartsActive = 20;
    public const int LayerPhysxPartTriggers = 21;
    public const int LayerPhysxScenery = 22;
    public const int LayerUiAdditional = 25;
    public const int LayerUiOab = 26;
    public const int LayerMap = 27;
    public const int LayerPhysxTerrain = 28;
    public const int LayerRenderDragRender = 29;
    public const int LayerRenderFlares = 30;
    public const int LayerPhysxInvisible = 31;
}
