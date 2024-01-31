using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using P1S.Common.Json;
using P1S.Common.Logging;
using P1S.Identifi.App.Service.Client.Services;
using P1S.Identifi.App.Service.Client.Utils;

namespace OpenTelemetryTest;

internal class Engine
{   private static QueueProcessor QueueProcessor;
    private static TracerProvider _tracer;

    public Engine()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
    }     private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
    {
        Console.WriteLine(unhandledExceptionEventArgs.ExceptionObject);
    }  
   
    public async Task OnStart()
    {
        _tracer=  InitOtel();
        var identifiServiceFactory = new IdentifiServiceFactory("https://dev-k8s-m1.dev.rph.int/mitch/identifiapi/", new ApiKeySecurityManager("12345"),
            new ConsoleLogger(new JsonSerializer()));
        var asyncService = new IdentifiClient2("https://dev-k8s-m1.dev.rph.int/mitch/identifiapi/", new ApiKeySecurityManager("12345"),
            new ConsoleLogger(new JsonSerializer()));
        QueueProcessor = new QueueProcessor(identifiServiceFactory,asyncService);
       
        await QueueProcessor.StartAsync();

    }
    private static TracerProvider InitOtel()
    {
        return  Sdk.CreateTracerProviderBuilder()
            .AddHttpClientInstrumentation( (options)=> options.EnrichWithHttpWebRequest  = (activity, httpRequestMessage) =>
            {
                  
                foreach (string key in httpRequestMessage.Headers.AllKeys)
                    activity.SetTag(key, httpRequestMessage.Headers[key]);

                  
            })
            .AddSqlClientInstrumentation(s =>
            {
                s.SetDbStatementForText = true;
                s.SetDbStatementForStoredProcedure = true;
            })
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService($"OTELTest-{DateTime.Now.ToString("g")}", serviceVersion:"1.0.0"))
            .AddSource(nameof(Program))
            //.AddConsoleExporter()
            .AddOtlpExporter(opts =>
            {
                opts.Endpoint =
                    new Uri(
                        $"http://localhost:4317");
                  
            })  
            .Build();
    }
}