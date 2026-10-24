#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Infrastructure.Settings;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ApplicationSettings : object
{
    #region Statics
    public static readonly string KeyName = nameof(ApplicationSettings);
    #endregion /Statics

    #region Constructor
    public ApplicationSettings() : base()
	{
        PageSettings = new();
        CultureSettings = new();
	}
    #endregion /Constructor

    #region Properties

    public PageSettings PageSettings { get; init; }
    public CultureSettings CultureSettings { get; set; }

    #endregion /Properties
}
