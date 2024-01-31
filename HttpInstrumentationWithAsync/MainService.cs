using System.Threading.Tasks;

namespace OpenTelemetryTest;

public class MainService
{
    private static Engine EngineService;
    public async Task Start()
    {
        if (EngineService == null)
        {
            EngineService = new Engine();
            await   EngineService.OnStart();
        }
    }


    public void Stop()
    { 
    }
}