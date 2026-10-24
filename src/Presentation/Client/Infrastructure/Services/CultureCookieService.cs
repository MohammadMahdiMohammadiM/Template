// **************************************************
using System;
using Microsoft.JSInterop;
using System.Globalization;
using System.Threading.Tasks;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components;
// **************************************************

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Infrastructure.Services;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class CultureCookieService
        (IJSRuntime js, ApplicationSettings applicationSettings, NavigationManager navigationManager)
{
    #region Statics
    private readonly static string CookieName = "Culture.Cookie";
    #endregion /Statics

    #region Properties

    #region private ApplicationSettings ApplicationSettings { get; set; } = applicationSettings;
    private ApplicationSettings ApplicationSettings { get; set; } = applicationSettings;
    #endregion /private ApplicationSettings ApplicationSettings { get; set; } = applicationSettings;

    #region private NavigationManager Navigation { get; set; } = navigationManager;
    private NavigationManager Navigation { get; set; } = navigationManager;
    #endregion /private NavigationManager Navigation { get; set; } = navigationManager;

    #endregion /Properties

    #region Methods

    #region public async Task<CultureInfo> GetCultureAsync()
    /// <summary>
    /// Reads the saved culture. Uses the default when the cookie is missing or not supported
    /// </summary>
    public async Task<CultureInfo> GetCultureAsync()
    {
        string? saved;
        try
        {
            saved = await js.InvokeAsync<string?>("portfolio.getCookie", CookieName);
        }
        catch (JSException)
        {
            // Cookies blocked or JavaScript not ready: use the default.
            Navigation.NavigateTo
                (Constants.CommonRouting.UnexpectedError, replace: true);

            return new CultureInfo(ApplicationSettings.CultureSettings.DefaultCulture!); ;
        }

        if (string.IsNullOrWhiteSpace(saved))
        {
            return new CultureInfo(ApplicationSettings.CultureSettings.DefaultCulture!);
        }
        else
        {
            return new CultureInfo(saved!);
        }
    }
    #endregion /public async Task<CultureInfo> GetCultureAsync()

    #region public async Task SetCultureAsync(string cultureName)
    /// <summary>
    /// Saves the chosen culture in the cookie. Refuses any culture that is not in the settings
    /// </summary>
    public async Task SetCultureAsync(string cultureName)
    {
        if (IsSupported(cultureName) is false)
        {
            throw new ArgumentException($"'{cultureName}' is not in SupportedCultures.", nameof(cultureName));
        }
        else
        {
            await CreateCookieAsync(cultureName);
        }
    }
    #endregion /public async Task SetCultureAsync(string cultureName)

    #region private bool IsSupported(string? name)
    private bool IsSupported(string? name)
    {
        if (name is null)
        {
            return false;
        }

        return ApplicationSettings.CultureSettings.SupportedCultures.Contains(name, StringComparer.OrdinalIgnoreCase);
    }
    #endregion /private bool IsSupported(string? name)

    #region public async Task ApplyCultureAsync()
    /// <summary>
    /// Reads the saved culture and makes it the culture for the whole app
    /// </summary>
    public async Task ApplyCultureAsync()
    {
        var culture = await GetCultureAsync();

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
    #endregion /public async Task ApplyCultureAsync()

    #region public async Task CreateCookieAsync(string cultureName)
    public async Task CreateCookieAsync(string cultureName)
    {
        var cookieOptions = new CookieOptions
        {
            Path = "/",

            Secure = true,

            HttpOnly = false,

            IsEssential = false,

            MaxAge = null,

            Expires = DateTimeOffset.UtcNow.AddYears(1),

            SameSite = SameSiteMode.Lax,
        };

        await js.InvokeVoidAsync("portfolio.deleteCookie", CookieName, "/");


        if (string.IsNullOrWhiteSpace(cultureName) is false)
        {
            await js.InvokeVoidAsync("portfolio.setCookieWithOptions", CookieName, cultureName, cookieOptions);
        }
    }
    #endregion /public async Task CreateCookieAsync(string cultureName)

    #endregion /Methods
}


