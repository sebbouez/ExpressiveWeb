// *********************************************************
// 
// ExpressiveWeb.Core Ser.cs
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

namespace ExpressiveWeb.Core.BackgroundServices;

public class BackgroundTaskManager : IBackgroundTaskManager
{
    public event EventHandler<BackgroundTaskManagerStatus>? StatusChanged;

    private BackgroundTaskManagerStatus _status = BackgroundTaskManagerStatus.Idle;

    public List<BackgroundTaskBase> Tasks
    {
        get;
    } = new();



    public void RunAsBackgroundTask(Action action)
    {
        Task.Run(action);
    }


    public void AddTask(BackgroundTaskBase task)
    {
        Tasks.Add(task);
        RunNextTask();
    }

    private void RunNextTask()
    {
        BackgroundTaskBase? next = Tasks.FirstOrDefault(t => !t.IsRunning);
        if (next is null)
        {
            _status = BackgroundTaskManagerStatus.Idle;
            return;
        }

        if (Tasks.Any(t => t.IsRunning && t.GetType() == next.GetType()))
        {
            return;
        }

        void OnCompleted(object? sender, EventArgs e)
        {
            next.Completed -= OnCompleted;
            Tasks.Remove(next);
            RunNextTask();
        }

        next.Completed += OnCompleted;
        
        _status = BackgroundTaskManagerStatus.Running;
        _ = next.RunAsync();
    }
}