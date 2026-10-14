namespace Constants;

public static class CommonRouting : object
{
    #region Statics

    #region Constructor
    static CommonRouting()
    {
    }
    #endregion /Constructor

    #endregion /Statics

    #region Contants
    public const string Home = "/";

    /// <summary>
	/// Error 404
	/// </summary>
    public const string NotFound = "/error404";

    public const string JustAView = "/just-a-view";

    public const string UsaFlag = "images/flags/us.svg";

    public const string UnexpectedError = "/unexpected-error";

    public const string IranFlag = "images/flags/lionsun.svg";

    public const string ChangeCulture = "/change-culture/{supportedCultureName}";

    #endregion /Contants
}
