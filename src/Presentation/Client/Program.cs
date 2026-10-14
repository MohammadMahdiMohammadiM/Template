// **************************************************
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
// **************************************************

namespace Client;

public class Program : object
{
    public Program() : base()
    {
    }

    public static async Task Main()
    {
        // **************************************************
        var builder = WebAssemblyHostBuilder.CreateDefault();
        // **************************************************

        // **************************************************
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        // **************************************************

        // **************************************************
        builder.Services.AddScoped
            (implementationFactory: current =>
            {
                var httpClient =
                new HttpClient
                {
                    BaseAddress =
                        new Uri(uriString: builder.HostEnvironment.BaseAddress),
                };

                return httpClient;
            });
        // **************************************************

        // **************************************************
        var applicationSettings =
            builder.Configuration.GetSection
            (ApplicationSettings.KeyName)
            .Get<ApplicationSettings>();
        // **************************************************

        // **************************************************
        builder.Services.Configure<ApplicationSettings>
          (builder.Configuration.GetSection(key: ApplicationSettings.KeyName))
        // **************************************************

        // **************************************************
        .AddSingleton
        (implementationFactory: serviceType =>
        {
            var result =
             serviceType.GetRequiredService
                <Microsoft.Extensions.Options.IOptions
                <ApplicationSettings>>().Value;

            return result;
        });
        // **************************************************

        // **************************************************
        builder.Services.AddLocalization();
        // **************************************************

        // **************************************************
        builder.Services.AddScoped<CultureCookieService>();
        // **************************************************

        // **************************************************
        var host = builder.Build();
        // **************************************************

        // **************************************************
        await host.Services.
            GetRequiredService<CultureCookieService>().ApplyCultureAsync();
        // **************************************************

        // **************************************************
        await host.RunAsync();

    }
}
