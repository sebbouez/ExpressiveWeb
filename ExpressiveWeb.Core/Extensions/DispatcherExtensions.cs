// *********************************************************
// 
// ExpressiveWeb.Core DispatcherExtensions.cs
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

using Avalonia.Threading;

namespace ExpressiveWeb.Core.Extensions;

public static class DispatcherExtensions
{
    private static readonly Dictionary<string, Action> QueuedDispatcherActions = new();

    private static readonly DispatcherTimer Timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(50)
    };

    /// <summary>
    /// Adds an action to the dispatcher queue associated with a given key, ensuring that it will be executed
    /// after a delay to prevent overlapping or redundant invocations.
    /// </summary>
    /// <param name="disp">The dispatcher instance on which the action will be queued.</param>
    /// <param name="key">The unique key used to identify and manage the queued action.</param>
    /// <param name="action">The action to be executed on the dispatcher.</param>
    public static void QueueAction(this Dispatcher disp, string key, Action action)
    {
        Timer.Stop();
        Timer.Tick -= TimerOnTick;
        QueuedDispatcherActions[key] = action;
        Timer.Tick += TimerOnTick;
        Timer.Start();
    }
    
    private static void TimerOnTick(object? sender, EventArgs e)
    {
        Timer.Stop();
        if (QueuedDispatcherActions.Count > 0)
        {
            foreach (KeyValuePair<string, Action> keyValuePair in QueuedDispatcherActions)
            {
                Timer.Dispatcher.Invoke(QueuedDispatcherActions[keyValuePair.Key]);
            }

            QueuedDispatcherActions.Clear();
        }
    }
}