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
	}
    #endregion /Constructor

    #region Properties

    #region public CultureSettings CultureSettings { get; set; } = default!;
    public CultureSettings CultureSettings { get; set; } = default!;
    #endregion /public CultureSettings CultureSettings { get; set; } = default!;

    #region public PageSettings PageSettings { get; set; } = default!;
    public PageSettings PageSettings { get; set; } = default!;
    #endregion /public PageSettings PageSettings { get; set; } = default!;

    #endregion /Properties
}
