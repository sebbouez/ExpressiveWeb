// *********************************************************
// 
// ExpressiveWeb.Core DefaultKnownHtmlElements.cs
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

namespace ExpressiveWeb.Core.Html;

public static class DefaultKnownHtmlElements
{
    public static List<HtmlElementDeclaration> KnownElements
    {
        get;
    } =
    [
        new HtmlElementDeclaration
        {
            Name = "Heading 1",
            TagName = "h1",
            Properties = new List<HtmlElementPropertyDeclaration>()
        },

        new HtmlElementDeclaration
        {
            Name = "Heading 2",
            TagName = "h2",
            Properties = new List<HtmlElementPropertyDeclaration>()
        },

        new HtmlElementDeclaration
        {
            Name = "Heading 3",
            TagName = "h3",
            Properties = new List<HtmlElementPropertyDeclaration>()
        },

        new HtmlElementDeclaration
        {
            Name = "Heading 4",
            TagName = "h4",
            Properties = new List<HtmlElementPropertyDeclaration>()
        },

        new HtmlElementDeclaration
        {
            Name = "Heading 5",
            TagName = "h5",
            Properties = new List<HtmlElementPropertyDeclaration>()
        },

        new HtmlElementDeclaration
        {
            Name = "Heading 6",
            TagName = "h6",
            Properties = new List<HtmlElementPropertyDeclaration>()
        }
    ];
}