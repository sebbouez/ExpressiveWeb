// *********************************************************
// 
// ExpressiveWeb.Core ContextCommandManager.cs
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

/// <summary>
/// Represents a branch of Undo/Redo manager, with a name. Used to switch between contexts of editing.
/// </summary>
internal class ContextCommandManager
{
    internal string ContextName
    {
        get;
        init;
    } = string.Empty;

    internal CommandStack RedoStack
    {
        get;
    } = new();

    internal CommandStack UndoStack
    {
        get;
    } = new();
}