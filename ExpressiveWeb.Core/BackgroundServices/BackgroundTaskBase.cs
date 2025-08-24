// *********************************************************
// 
// ExpressiveWeb.Core BackgroundTaskBase.cs
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

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ExpressiveWeb.Core.BackgroundServices;

public abstract class BackgroundTaskBase : INotifyPropertyChanged
{
    private bool _hasErrors;
    private bool _isRunning;
    private string? _lastMessage;

    public event EventHandler? Completed;
    public event EventHandler<string?>? MessageRaised;
    public event EventHandler<string?>? ErrorRaised;
    public event PropertyChangedEventHandler? PropertyChanged;


    public string? LastMessage
    {
        get
        {
            return _lastMessage;
        }
        set
        {
            _lastMessage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastMessage)));
        }
    }

    public bool HasErrors
    {
        get
        {
            return _hasErrors;
        }
        set
        {
            _hasErrors = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasErrors)));
        }
    }

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        private set
        {
            _isRunning = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
        }
    }

    public ObservableCollection<BackgroundTaskLogItem> Logs
    {
        get;
        set;
    } = new();

    private void AddLog(BackgroundTaskLogItem logItem)
    {
        Logs.Add(logItem);

        LastMessage = logItem.Message;
        HasErrors = HasErrors || logItem.Level == BackgroundTaskLogLevel.Error;
    }

    protected abstract void InternalRun();

    public Task RunAsync()
    {
        IsRunning = true;
        return Task.Run(() =>
        {
            try
            {
                InternalRun();
            }
            catch (Exception e)
            {
                AddLog(new BackgroundTaskLogItem
                {
                    Time = DateTime.Now,
                    Level = BackgroundTaskLogLevel.Error,
                    Message = e.Message
                });
            }
            finally
            {
                IsRunning = false;
                Completed?.Invoke(this, EventArgs.Empty);
            }
        });
    }
}