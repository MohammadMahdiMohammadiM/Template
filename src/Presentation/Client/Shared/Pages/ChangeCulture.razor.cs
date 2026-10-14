// **************************************************
// **************************************************
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
// **************************************************


namespace Client.Shared.Pages;

public partial class ChangeCulture()
{
    #region Properties

    #region public string? ReturnUrl { get; set; }
    [SupplyParameterFromQuery]
    public string? ReturnUrl { get; set; }
    #endregion /public string? ReturnUrl { get; set; }

    #region public string? SupportedCultureName { get; set; }
    [Parameter]
    public string? SupportedCultureName { get; set; }
    #endregion /public string? SupportedCultureName { get; set; }

    #endregion /Properties

    #region Methods

    #region protected override async Task OnInitializedAsync()
    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(SupportedCultureName) is false)
        {
            try
            {
                await CultureCookie.SetCultureAsync(SupportedCultureName);
            }
            catch(Exception)
            {
                Navigation.NavigateTo
                    (Constants.CommonRouting.UnexpectedError, replace: true);
                return;
            }
        }

        if (string.IsNullOrWhiteSpace(ReturnUrl) is false
            &&
            ReturnUrl.StartsWith(Navigation.BaseUri, StringComparison.OrdinalIgnoreCase))
        {
            Navigation.NavigateTo(uri: ReturnUrl, forceLoad: true);
        }
        else
        {
            Navigation.NavigateTo(uri: Navigation.BaseUri, forceLoad: true);
        }
    }
    #endregion /protected override async Task OnInitializedAsync()

    #endregion /Methods
}
