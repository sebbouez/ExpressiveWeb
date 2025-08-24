// *********************************************************
// 
// ExpressiveWeb.Core CommandStack.cs
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

using System.Linq;

namespace ExpressiveWeb.Core.Commands;

/// <summary>
/// Represents a command stack that stores <see cref="IBusinessCommand"/>
/// </summary>
internal class CommandStack
{
    private readonly List<IBusinessCommand> _items = new();

    internal int Count
    {
        get
        {
            return _items.Count;
        }
    }

    internal void Clear()
    {
        _items.Clear();
    }

    internal IEnumerator<IBusinessCommand> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    internal IBusinessCommand? Pop()
    {
        if (_items.Count > 0)
        {
            IBusinessCommand temp = _items[^1];
            _items.RemoveAt(_items.Count - 1);
            return temp;
        }

        return null;
    }

    internal void Push(IBusinessCommand item)
    {
        _items.Add(item);
    }

    internal void Remove(int itemAtPosition)
    {
        _items.RemoveAt(itemAtPosition);
    }
}