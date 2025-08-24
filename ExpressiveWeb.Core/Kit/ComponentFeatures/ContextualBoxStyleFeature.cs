// *********************************************************
// 
// ExpressiveWeb.Core ContextualBoxStyleFeature.cs
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

using System.Xml;

namespace ExpressiveWeb.Core.Kit.ComponentFeatures;

public class ContextualBoxStyleFeature : ComponentFeatureBase
{
    /// <summary>
    /// Indicates whether the user can change the border settings
    /// </summary>
    public bool EnableBorderSettings
    {
        get;
        private set;
    }

    /// <summary>
    /// Indicates whether the user can change the background settings
    /// </summary>
    public bool EnableBackgroundSettings
    {
        get;
        private set;
    }

    public override void Init(XmlNode node)
    {
        if (node.Attributes == null)
        {
            return;
        }

        XmlAttribute? borderAttr = node.Attributes["EnableBorderSettings"];
        XmlAttribute? backgroundAttr = node.Attributes["EnableBackgroundSettings"];

        EnableBorderSettings = borderAttr != null && bool.Parse(borderAttr.Value);
        EnableBackgroundSettings = backgroundAttr != null && bool.Parse(backgroundAttr.Value);
    }
}