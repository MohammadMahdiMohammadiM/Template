#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Infrastructure.Settings;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class CultureSettings : object
{
    #region Constructor
    public CultureSettings() : base()
    {
    }
    #endregion /Constructor

    #region Properties

    #region public string[]? SupportedCultures { get; set; }
    public string[]? SupportedCultures { get; set; }
    #endregion /public string[]? SupportedCultures { get; set; }

    #region public string? DefaultCulture { get; set; }
    public string? DefaultCulture { get; set; }
    #endregion /public string? DefaultCulture { get; set; }

    #endregion /Properties

}
