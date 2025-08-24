// *********************************************************
// 
// ExpressiveWeb.Core LibraryImageItem.cs
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

namespace ExpressiveWeb.Core.Libraries;

using System.Xml.Linq;

public class LibraryImageItem : LibraryItemBase
{
    public string FileName
    {
        get;
        private set;
    } = string.Empty;

    internal override void Load(XDocument doc)
    {
        XElement root = doc.Root ?? throw new InvalidOperationException("Invalid library image XML: missing root element");
        Name = root.Element("Name")?.Value?.Trim() ?? string.Empty;
        XElement? fileEl = root.Element("File");
        if (fileEl != null)
        {
            FileName = fileEl.Attribute("Name")?.Value?.Trim() ?? string.Empty;
        }
    }

    protected override void ImportToProjectCollection(Project.Project project)
    {
        throw new NotImplementedException();
    }
}