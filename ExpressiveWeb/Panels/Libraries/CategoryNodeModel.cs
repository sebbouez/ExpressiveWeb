// *********************************************************
// 
// ExpressiveWeb CategoryNodeModel.cs
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
using ExpressiveWeb.Core.Libraries;

namespace ExpressiveWeb.Panels.Libraries;

public class CategoryNodeModel
{
    public CategoryNodeModel(string name, CategoryNodeType type, int count)
    {
        Name = name;
        Type = type;
        Count = count;
    }

    public string Name
    {
        get;
    }

    public CategoryNodeType Type
    {
        get;
    }

    public int Count
    {
        get;
    }

    // Collection de nœuds enfants pour une structure hiérarchique
    public ObservableCollection<CategoryNodeModel> Children
    {
        get;
    } = new();

    public string DisplayText
    {
        get
        {
            return $"{Name} ({Count})";
        }
    }
}