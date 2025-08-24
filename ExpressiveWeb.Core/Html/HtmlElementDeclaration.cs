// *********************************************************
// 
// ExpressiveWeb.Core HtmlElementDeclaration.cs
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

public class HtmlElementDeclaration
{
    /// <summary>
    /// Gets the name of the Html element.
    /// </summary>
    public string Name
    {
        get;
        internal set;
    } = null!;

    /// <summary>
    /// Gets the Html tag name.
    /// </summary>
    public string TagName
    {
        get;
        internal set;
    } = null!;

    /// <summary>
    /// Gets the list of properties.
    /// </summary>
    public List<HtmlElementPropertyDeclaration> Properties
    {
        get;
        init;
    } = new();
}