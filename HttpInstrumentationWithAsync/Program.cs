using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HttpInstrumentationWithAsync
{
    public class Program
    {
        private static readonly WorkerClass WorkerClass;
        private static readonly List<Activity> Activities;
        public static List<Activity> AsyncActivities;
        public static List<Activity> SyncActivities;

        static Program()
        {
            Activities = new List<Activity>();
            WorkerClass = new WorkerClass();
        }
        public static async Task Main(string[] args)
        {
           
            using (var  tracer = InitializeOpenTelemetry())
            {
                
                if ((args.Length>0 && args[0] == "async" )|| args.Length==0)
                {
                    await WorkerClass.StartAsync();
                    AsyncActivities = new List<Activity>(Activities);
                }
                if (args.Length>0 && args[0] == "sync")
                {
                    WorkerClass.StartSync();
                    SyncActivities =  new List<Activity>(Activities);
                }
              
                Activities.Clear();
              
            }
        }


  
        private static TracerProvider InitializeOpenTelemetry()
        {
            return Sdk.CreateTracerProviderBuilder()
                .AddHttpClientInstrumentation()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Program", serviceVersion: "1.0.0"))
                .AddSource(nameof(Program)).AddConsoleExporter()
                .AddInMemoryExporter(Activities)
                .AddOtlpExporter(s=>s.Endpoint = new Uri(  $"http://localhost:4317")) //uncomment to send traces to Jaeger All in One
                .Build();
        }
    }
}