// *********************************************************
// 
// ExpressiveWeb.Core ApplicationCommandsManager.cs
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

namespace ExpressiveWeb.Core.ApplicationCommands;

public class ApplicationCommandsService : IApplicationCommandsService
{
    private HashSet<ApplicationCommandBase> _commandsCache = new();

    public List<ApplicationCommandBase> RegisteredCommands
    {
        get
        {
            return _commandsCache.ToList();
        }
    }

    public void RegisterCommands(List<ApplicationCommandBase> commands)
    {
        void RegisterSubCommands(ApplicationCommandBase command)
        {
            if (command.SubCommands == null) return;

            foreach (var subCommand in command.SubCommands)
            {
                _commandsCache.Add(subCommand);
                RegisterSubCommands(subCommand);
            }
        }

        foreach (var command in commands)
        {
            _commandsCache.Add(command);
            RegisterSubCommands(command);
        }
    }

    public void RegisterCommand(ApplicationCommandBase command)
    {
        _commandsCache.Add(command);
    }

    public T? GetCommand<T>() where T : ApplicationCommandBase
    {
        ApplicationCommandBase? command = _commandsCache.FirstOrDefault(x => x.GetType().Equals(typeof(T)));

        if (command is T a) return a;

        if (command == null)
        {
            command = Activator.CreateInstance<T>();
            _commandsCache.Add(command);

            return (T) command;
        }

        return null;
    }

    public void SetCommandState<T>(bool state)
    {
        if (typeof(T).IsInterface)
        {
            foreach (ApplicationCommandBase command in _commandsCache.Where(x =>
                         x.GetType().GetInterfaces().Contains(typeof(T)))) command.IsEnabled = state;
        }
        else
        {
            foreach (ApplicationCommandBase command in _commandsCache.Where(x =>
                         x.GetType() == typeof(T)))

            {
                command.IsEnabled = state;
            }
        }
    }
}