// **************************************************
using System.Globalization;
using System.Threading.Tasks;
// **************************************************

namespace Client;

public partial class App
{
    #region Constructor
    public App() : base()
    {
    }
    #endregion /Constructor

    #region Properties

    #region private static string Lang { get; set; } = string.Empty;
    private static string Lang { get; set; } = string.Empty;
    #endregion /private static string Lang { get; set; } = string.Empty;

    #region private static string Direction { get; set; } = string.Empty;
    private static string Direction { get; set; } = string.Empty;
    #endregion /private static string Direction { get; set; } = string.Empty;

    #endregion /Properties

    #region Methods

    #region protected override async Task OnInitializedAsync()
    protected override async Task OnInitializedAsync()
    {
        if (CultureInfo.CurrentUICulture.Parent.NativeName is Constants.CommonName.Farsi)
        {
            Lang = "fa";
            Direction = "rtl";
        }
        else
        {
            Lang = "en";
            Direction = "ltr";
        }

        await Task.CompletedTask;
    }
    #endregion /protected override async Task OnInitializedAsync()

    #endregion /Methods
}
