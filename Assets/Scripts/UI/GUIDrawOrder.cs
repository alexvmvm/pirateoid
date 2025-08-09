
/// <summary>
/// Higher means it receives gui events first.
/// </summary>
public static class GUIDrawOrder
{
    public const int Default = 0;
    public const int Window = 500;
    public const int FloatMenu = 1000;
    public const int Tooltip = 1500;
    public const int Map = 2000;
}