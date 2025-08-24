// *********************************************************
// 
// ExpressiveWeb.Core ProjectItem.cs
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

namespace ExpressiveWeb.Core.Project;

public class ProjectItem
{
    public string Name
    {
        get;
        set;
    } = null!;

    public ProjectItemType ItemType
    {
        get;
        set;
    }

    public string? Path
    {
        get;
        set;
    }

    public ObservableCollection<ProjectItem> Children
    {
        get;
        set;
    } = new ObservableCollection<ProjectItem>();
}