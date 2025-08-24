// *********************************************************
// 
// ExpressiveWeb.Core BusinessCommandManager.cs
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

using ExpressiveWeb.Core.Log;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressiveWeb.Core.Commands;

/// <summary>
/// Represents the undo/redo manager
/// </summary>
public class BusinessCommandManager : IBusinessCommandManager
{
    private readonly List<ContextCommandManager> _contextManagers = new();

    private readonly ILogService _logService;

    public BusinessCommandManager()
    {
        _logService = AppServices.ServicesFactory!.GetService<ILogService>()!;
        _contextManagers.Add(new ContextCommandManager {ContextName = ""});
    }

    public bool CanRedo
    {
        get
        {
            return _contextManagers.Last().RedoStack.Count > 0;
        }
    }

    public bool CanUndo
    {
        get
        {
            return _contextManagers.Last().UndoStack.Count > 0;
        }
    }

    public bool HasCommands
    {
        get
        {
            return CanUndo || CanRedo;
        }
    }

 

    public bool IsReadonly
    {
        get;
        set;
    }

    public IBusinessCommand? LastCommand
    {
        get;
        private set;
    }

    public string CurrentContext
    {
        get
        {
            return _contextManagers.Last().ContextName;
        }
    }

    public event EventHandler<bool>? Changed;

    public void CloseContext(string contextName)
    {
        ContextCommandManager? contextStack = _contextManagers.LastOrDefault(x => x.ContextName == contextName);
        if (contextStack != null)
        {
            _contextManagers.Remove(contextStack);
        }

        Changed?.Invoke(this, false);
    }

    public void ExecuteCommand(IBusinessCommand cmd)
    {
        try
        {
            if (IsReadonly)
            {
                return;
            }

            cmd.Do();
            _contextManagers.Last().UndoStack.Push(cmd);
            _contextManagers.Last().RedoStack.Clear(); // Once we issue a new command, the redo stack clears
            Changed?.Invoke(this, false);
        }
        catch (Exception e)
        {
            _logService.Error(e);
        }
    }

    public void ExecuteCommandSilent(IBusinessCommand cmd)
    {
        try
        {
            if (IsReadonly)
            {
                return;
            }

            _contextManagers.Last().UndoStack.Push(cmd);
            _contextManagers.Last().RedoStack.Clear();
            Changed?.Invoke(this, false);
        }
        catch (Exception e)
        {
            _logService.Error(e);
        }
    }

    public void Redo()
    {
        if (IsReadonly)
        {
            return;
        }

        if (_contextManagers.Last().RedoStack.Count > 0)
        {
            IBusinessCommand? cmd = _contextManagers.Last().RedoStack.Pop();
            LastCommand = cmd;
            if (cmd != null)
            {
                cmd.Do();
                _contextManagers.Last().UndoStack.Push(cmd);
            }
        }

        Changed?.Invoke(this, true);
    }

    public void Reset()
    {
        if (IsReadonly)
        {
            return;
        }

        _contextManagers.Last().RedoStack.Clear();
        _contextManagers.Last().UndoStack.Clear();
        Changed?.Invoke(this, false);
    }

    public void OpenContext(string contextName)
    {
        _contextManagers.Add(new ContextCommandManager
        {
            ContextName = contextName
        });

        Changed?.Invoke(this, false);
    }

    public void Undo()
    {
        if (IsReadonly)
        {
            return;
        }

        if (_contextManagers.Last().UndoStack.Count > 0)
        {
            IBusinessCommand? cmd = _contextManagers.Last().UndoStack.Pop();
            LastCommand = cmd;
            if (cmd != null)
            {
                cmd.Undo();
                _contextManagers.Last().RedoStack.Push(cmd);
            }
        }

        Changed?.Invoke(this, true);
    }
}