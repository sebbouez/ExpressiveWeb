// *********************************************************
// 
// ExpressiveWeb.Designer DynamicResourcesRouter.cs
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

using System.Text;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Core.Kit.ComponentFeatures;
using ExpressiveWeb.Designer.Extensions;

namespace ExpressiveWeb.Designer.Cef.Impl;

public class DynamicResourcesRouter
{
    private string BuildEditorScript()
    {
        StringBuilder sb = new();
        StringBuilder sbComponents = new();
        StringBuilder sbDecoratorsComponents = new();

        sb.AppendLine("var $EDITOR_KIT_DATA = {");

        sb.Append("COMPONENTS_CATALOG: [");

        foreach (KitComponent component in HtmlEditor.Kit.Components)
        {
            if (!string.IsNullOrEmpty(component.Slots))
            {
                string[] slots = component.Slots.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (string slot in slots)
                {
                    sbDecoratorsComponents.Append($"{component.GetSelector()} {slot},");
                }
            }

            sbComponents.Append($"{component.GetSelector()},");
            sbDecoratorsComponents.Append($"{component.GetSelector()},");

            sb.AppendLine("{");
            sb.AppendLine($"uid: '{component.UID}',");
            sb.AppendLine($"allowsInlineEditing: {component.HasFeature<InlineEditFeature>().ToJsFormat()},");
            sb.AppendLine($"hasContextualActions: {component.ActionList.Any().ToJsFormat()},");

            sb.AppendLine($"selector: '{component.GetSelector()}',");

            sb.AppendLine("accepts: [");

            foreach (string s in component.Accepts.Split(';'))
            {
                GetComponentsInFamily(s, sb);
            }

            sb.AppendLine("],");
            sb.AppendLine("denies: [");

            foreach (string s in component.Denies.Split(';'))
            {
                sb.AppendLine($"'{s.Trim()}',");
            }

            sb.AppendLine("],");


            sb.AppendLine("slots: [");

            foreach (string s in component.Slots.Split(';'))
            {
                sb.AppendLine($"'{s.Trim()}',");
            }

            sb.AppendLine("],");


            sb.AppendLine("},");
        }

        sb.Append("],");


        sb.AppendLine($"KNOWN_COMPONENTS_SELECTOR: \"{sbComponents.ToString().TrimEnd(',')}\", ");
        sb.AppendLine($"ADORNER_DECORATORS_SELECTOR: \"{sbDecoratorsComponents.ToString().TrimEnd(',')}\" ");

        sb.AppendLine("}");


        sb.AppendLine("document.addEventListener('DOMContentLoaded', (event) => {");
        sb.AppendLine($"{HtmlEditor.JS_GLOBAL_EDITOR_OBJ_NAME} = new EditorComponent();");
        sb.AppendLine("});");


        return sb.ToString();
    }

    public InternalRouterResult Execute(string path)
    {
        InternalRouterResult result = new();

        if (path.EndsWith("kit-utils.js", StringComparison.OrdinalIgnoreCase))
        {
            string js = BuildEditorScript();
            result.Content = Encoding.UTF8.GetBytes(js);
        }

        result.MimeType = "text/javascript";
        return result;
    }

    private void GetComponentsInFamily(string value, StringBuilder sb)
    {
        List<KitComponent> foundComponents = HtmlEditor.Kit.Components.Where(c => c.Family.Equals(value, StringComparison.Ordinal)).ToList();

        if (foundComponents.Count != 0)
        {
            foreach (KitComponent component in foundComponents)
            {
                sb.AppendLine($"'{component.UID}',");
            }
        }
        else
        {
            sb.AppendLine($"'{value.Trim()}',");
        }
    }
}