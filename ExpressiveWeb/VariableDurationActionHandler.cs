// *********************************************************
// 
// ExpressiveWeb VariableDurationActionHandler.cs
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

using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using ExpressiveWeb.CommonDialogs;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Log;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb;

public class VariableDurationActionHandler : IDisposable
{
    private readonly DispatcherTimer _timer;
    private readonly LongOperationDialog _dialog;
    private readonly ILogService _logService;

    public VariableDurationActionHandler(double maxDurationBeforeNotify)
    {
        _logService = AppServices.ServicesFactory!.GetService<ILogService>()!;

        _dialog = new LongOperationDialog();
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(maxDurationBeforeNotify)
        };
        _timer.Tick += Timer_Tick;
    }

    public void Dispose()
    {
        Abort();
    }

    private void Abort()
    {
        Dispatcher.UIThread.Post(() =>
        {
            _dialog?.Close();
        });

        if (_timer.IsEnabled)
        {
            _timer.Stop();
        }
    }

    public async Task Run(Action action)
    {
        _timer.IsEnabled = true;
        _timer.Start();

        await Task.Run(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Abort();
                _logService.Error(ex);
                throw;
            }
        });
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _dialog?.Show(AppState.Instance.AppWindow);
        _timer.Stop();
    }
}