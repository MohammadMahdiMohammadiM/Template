// **************************************************
// **************************************************
using System;
using ViewModels;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Components;
// **************************************************

namespace Client.Shared.Pages;

public partial class
    Home(IJSRuntime jS, ApplicationSettings applicatonSettings) : IAsyncDisposable
{
    #region Properties

    #region private IJSRuntime JS { get; set; } = jS;
    [Inject]
    private IJSRuntime JS { get; set; } = jS;
    #endregion /private IJSRuntime JS { get; set; } = jS;

    #region public ApplicationSettings ApplicatonSettings { get; set; } = applicatonSettings;
    public ApplicationSettings ApplicatonSettings { get; set; } = applicatonSettings;
    #endregion /public ApplicationSettings ApplicatonSettings { get; set; } = applicatonSettings;

    #region private NavigationManager Navigation { get; set; } = default!;
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;
    #endregion /private NavigationManager Navigation { get; set; } = default!;

    #region private int Rotate { get; set; }
    /// <summary>
    /// An optional feature that allows users to flip cards
    /// </summary>
    private int Rotate { get; set; }

    #endregion /private int Rotate { get; set; }

    #region private System.Collections.Generic.IList<RoadMapViewModel> StepViewModel { get; set; }
    /// <summary>
    /// a collection which is using a record as output and show the fix data about career path
    /// </summary>
    private System.Collections.Generic.IList<RoadMapViewModel>? RoadMapViewModel { get; set; }
    #endregion /private System.Collections.Generic.IList<RoadMapViewModel> StepViewModel { get; set; }

    #region private System.Collections.Generic.IList<string> Skills { get; set; }
    /// <summary>
    /// a collection which is using string as output and show the skills (skills are not fix data)
    /// </summary>
    private System.Collections.Generic.IList<string>? Skills { get; set; }
    #endregion /private System.Collections.Generic.IList<string> Skills { get; set; }

    #region private System.Collections.Generic.IList<SocialViewModel> SocialViewModel { get; set; }
    /// <summary>
    /// a collection which is using a record as output and show the fix data about social links
    /// </summary>
    private System.Collections.Generic.IList<SocialViewModel>? SocialViewModel { get; set; }
    #endregion /private System.Collections.Generic.IList<SocialViewModel> SocialViewModel { get; set; }

    #endregion /Properties

    #region Methods

    #region private void Flip()
    /// <summary>
    /// Flipping the Card
    /// </summary>
    private void Flip()
    {
        try
        {
            if (ApplicatonSettings.PageSettings.RevealOnScroll)
            {
                Rotate += ApplicatonSettings.PageSettings.Rotate;
            }
        }
        catch(Exception)
        {
            Navigation.NavigateTo
                (Constants.CommonRouting.UnexpectedError, replace: true);
        }
    }
    #endregion /private void Flip()

    #region protected override async Task OnInitializedAsync()
    /// <summary>
    /// Datas Loads Once on this method
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        try
        {
            RoadMapViewModel =
            [
                new((Resources.RoadMap.Years.Year2021), (Resources.RoadMap.RoadMaps.Year2021Section1), (Resources.RoadMap.RoadMaps.Year2021Section2)),
                new((Resources.RoadMap.Years.Year2022Year2024), (Resources.RoadMap.RoadMaps.Year2022Year2024Section1), (Resources.RoadMap.RoadMaps.Year2022Year2024Section2)),
                new((Resources.RoadMap.Years.Year2025Year2026), (Resources.RoadMap.RoadMaps.Year2025Year2026Section1), (Resources.RoadMap.RoadMaps.Year2025Year2026Section2))
            ];

            SocialViewModel =
            [
                new(Resources.DataDictionary.Telegram, "https://t.me/ArtanArian"),
                new(Resources.DataDictionary.Reddit, "https://www.reddit.com/user/ArtanArian"),
                new(Resources.DataDictionary.GitHub, "https://github.com/MohammadMahdiMohammadiM"),
                new(Resources.DataDictionary.LinkedIn, "https://www.linkedin.com/in/mohammad-mahdi-mohammadi-m"),
                new(Resources.DataDictionary.StackOverflow, "https://stackoverflow.com/users/17306200/mohammad-mahdi-mohammadi-m"),
            ];

            Skills =
            [
                (Resources.Skill.Skills.CSharp),
                (Resources.Skill.Skills.Python),
                (Resources.Skill.Skills.ASPDotNETCore),
                (Resources.Skill.Skills.BootStrap),
                (Resources.Skill.Skills.HtmlCssJs),
                (Resources.Skill.Skills.EntityFrameworkCoreAndLINQ),
                (Resources.Skill.Skills.SQLServer),
                (Resources.Skill.Skills.BlazorWebAssembly),
                (Resources.Skill.Skills.Git),
            ];
        }
        catch(Exception)
        {
            Navigation.NavigateTo
                (Constants.CommonRouting.UnexpectedError, replace: true);
            return;
        }

        await Task.CompletedTask;
    }
    #endregion /protected override async Task OnInitializedAsync()

    #region protected override async Task OnAfterRenderAsync(bool firstRender)
    /// <summary>
    /// Loading JavaScripts
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("portfolio.init");
            }
        }
        catch
        {
            Navigation.NavigateTo
                (Constants.CommonRouting.UnexpectedError, replace: true);
            return;
        }

        await Task.CompletedTask;
    }
    #endregion /protected override async Task OnAfterRenderAsync(bool firstRender)

    #region public async ValueTask DisposeAsync()
    /// <summary>
    /// Cleaning the memory
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        try
        {
            await JS.InvokeVoidAsync("portfolio.destroy");

            GC.SuppressFinalize(this);
        }
        catch(Exception)
        {
            Navigation.NavigateTo
                (Constants.CommonRouting.UnexpectedError, replace: true);
            return;
        }
    }
    #endregion /public async ValueTask DisposeAsync()

    #endregion /Methods
}
