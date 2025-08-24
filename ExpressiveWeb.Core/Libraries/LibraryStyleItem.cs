// *********************************************************
// 
// ExpressiveWeb.Core LibraryStyleItem.cs
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

public class LibraryStyleItem : LibraryItemBase
{
    public string CssClassName
    {
        get;
        private set;
    } = string.Empty;

    public string CssContent
    {
        get;
        private set;
    } = string.Empty;

    internal override void Load(XDocument doc)
    {
        XElement root = doc.Root ?? throw new InvalidOperationException("Invalid library style XML: missing root element");
        Name = root.Element("Name")?.Value?.Trim() ?? string.Empty;
        XElement? cssClassEl = root.Element("CssClass");

        if (cssClassEl == null)
        {
            return;
        }

        CssClassName = cssClassEl.Attribute("Name")?.Value?.Trim() ?? string.Empty;
        // Inner text may be within CDATA; Value will return the content
        CssContent = (cssClassEl.Value ?? string.Empty).Trim();
    }

    protected override void ImportToProjectCollection(Project.Project project)
    {
        throw new NotImplementedException();
    }
}