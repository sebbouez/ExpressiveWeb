// *********************************************************
// 
// ExpressiveWeb.Core LogService.cs
// Copyright (c) Sébastien Bouez. All rights reserved.
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// *********************************************************

using ExpressiveWeb.Core.Env;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace ExpressiveWeb.Core.Log;

public class LogService : ILogService
{
    private readonly Logger _log;

    public LogService(IEnvironmentService environment)
    {
        _log = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(environment.ApplicationDataFolder, "logs", "logs-.txt"),
                retainedFileCountLimit: 10,
                fileSizeLimitBytes: 10000000,
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public void Info(string message)
    {
        _log.Information(message);
    }

    public void Warning(string message)
    {
        _log.Warning(message);
    }

    public void Error(string message)
    {
        _log.Error(message);
    }

    public void Error(Exception ex)
    {
        _log.Error(ex, "{Message}", ex.Message);
    }

    public void Dispose()
    {
        _log.Dispose();
    }
}