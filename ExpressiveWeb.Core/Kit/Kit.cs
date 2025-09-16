// *********************************************************
// 
// ExpressiveWeb.Core Kit.cs
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

using ExpressiveWeb.Common.Packages.Providers;
using ExpressiveWeb.Core.Html;

namespace ExpressiveWeb.Core.Kit;

public class Kit
{
    public Kit(string name)
    {
        KnownElements = DefaultKnownHtmlElements.KnownElements;
        Name = name;
    }

    /// <summary>
    ///     Gets the internal name of the Kit
    /// </summary>
    public string Name
    {
        get;
        private set;
    }

    /// <summary>
    ///     Gets the display name of the Kit
    /// </summary>
    public string DisplayName
    {
        get;
        set;
    }

    public Version Version
    {
        get;
        set;
    }

    /// <summary>
    ///     Gets the list of native html elements known by this kit.
    /// </summary>
    public List<HtmlElementDeclaration> KnownElements
    {
        get;
    } = new();

    public List<KitComponent> Components
    {
        get;
        set;
    } = new(); 
    
    public List<KitGalleryItem> GalleryItems
    {
        get;
        set;
    } = new();

    public List<KitPageTemplate> Templates
    {
        get;
        set;
    } = new();

    /// <summary>
    ///     Gets the list of default package references required by this kit.
    /// </summary>
    public List<PackageReference> DefaultPackages
    {
        get;
    } = new();
}