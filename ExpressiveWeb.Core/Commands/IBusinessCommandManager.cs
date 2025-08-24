// *********************************************************
// 
// ExpressiveWeb.Core IBusinessCommandManager.cs
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

namespace ExpressiveWeb.Core.Commands;

public interface IBusinessCommandManager
{
    /// <summary>
    /// Indicates if the CommandManager can operate an Undo action
    /// </summary>
    bool CanRedo
    {
        get;
    }

    /// <summary>
    /// Indicates if the CommandManager can operate an Redo action
    /// </summary>
    bool CanUndo
    {
        get;
    }

    /// <summary>
    /// Indicates if the CommandManager has commands in its current stack
    /// </summary>
    bool HasCommands
    {
        get;
    }

    /// <summary>
    /// Defines if the CommandManager should store commands or not
    /// </summary>
    bool IsReadonly
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the last command in the current context.
    /// </summary>
    IBusinessCommand? LastCommand
    {
        get;
    }

    /// <summary>
    /// Gets the current context name
    /// </summary>
    string CurrentContext
    {
        get;
    }

    /// <summary>
    /// Ocures when the anything happened, such as Do, Undo, Redo.
    /// </summary>
    event EventHandler<bool>? Changed;

    /// <summary>
    /// Closes a context. Exits the branch of temporary commands and keeps only the latest state in the main branch of undoable commands.
    /// </summary>
    /// <param name="contextName"></param>
    void CloseContext(string contextName);

    /// <summary>
    /// Runs a command in the current context. Performs <see cref="IBusinessCommand.Do"/> and add it to the stack of undoable commands.
    /// </summary>
    /// <param name="cmd">The command to execute</param>
    void ExecuteCommand(IBusinessCommand cmd);

    /// <summary>
    /// Runs a command in the current context. Adds a command to the stack of undoable commands, but DOES NOT execute it.
    /// Use it when your workflow did something complex and you want to be able to reverse it.
    /// </summary>
    /// <param name="cmd">The command to execute</param>
    void ExecuteCommandSilent(IBusinessCommand cmd);

    /// <summary>
    /// Opens a new context
    /// </summary>
    /// <param name="contextName"></param>
    void OpenContext(string contextName);

    /// <summary>
    /// Performs a Redo action on the latest command
    /// </summary>
    void Redo();

    /// <summary>
    /// Clears the current context command stack
    /// </summary>
    void Reset();

    /// <summary>
    /// Performs an Undo action on the latest command
    /// </summary>
    void Undo();
}