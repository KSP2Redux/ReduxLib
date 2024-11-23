using System;

namespace ReduxLib.GameInterfaces;

public interface ILocalizer
{
    public static ILocalizer Instance;
    public event Action OnLocalize;

    public string? GetTranslation(string key, params object[] p);

    public void AddCsvSource(string csv);
    public void AddI2CsvSource(string i2Csv);
}