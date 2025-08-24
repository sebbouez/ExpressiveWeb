// *********************************************************
// 
// ExpressiveWeb.Core LibraryPaletteItem.cs
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

public class LibraryPaletteItem : LibraryItemBase
{
    public sealed class PaletteColor
    {
        public string Name
        {
            get;
            init;
        } = string.Empty;

        public string Value
        {
            get;
            init;
        } = string.Empty;

        public string VarName
        {
            get;
            init;
        } = string.Empty;
    }

    public List<PaletteColor> Colors
    {
        get;
    } = new();

    internal override void Load(XDocument doc)
    {
        XElement root = doc.Root ?? throw new InvalidOperationException("Invalid library palette XML: missing root element");
        Name = root.Element("Name")?.Value?.Trim() ?? string.Empty;
        XElement? colorsEl = root.Element("Colors");

        if (colorsEl == null)
        {
            return;
        }

        foreach (XElement colorEl in colorsEl.Elements("Color"))
        {
            Colors.Add(new PaletteColor
            {
                Name = colorEl.Attribute("Name")?.Value.Trim() ?? string.Empty,
                Value = colorEl.Attribute("Value")?.Value.Trim() ?? string.Empty,
                VarName = colorEl.Attribute("VarName")?.Value.Trim() ?? string.Empty
            });
        }
    }

    protected override void ImportToProjectCollection(Project.Project project)
    {
        throw new NotImplementedException();
    }
}