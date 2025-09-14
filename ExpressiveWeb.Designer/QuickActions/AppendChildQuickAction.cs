// *********************************************************
// 
// ExpressiveWeb.Designer AppendChildQuickAction.cs
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

using ExpressiveWeb.Core.Html;
using ExpressiveWeb.Core.Kit;
using ExpressiveWeb.Designer.Commands;
using ExpressiveWeb.Designer.Exceptions;
using ExpressiveWeb.Designer.Filters;
using ExpressiveWeb.Designer.Models;

namespace ExpressiveWeb.Designer.QuickActions;

public class AppendChildQuickAction : IEditorQuickAction
{
    public string CommandName
    {
        get
        {
            return "AppendChild";
        }
    }

    public void Execute(HtmlEditor editor, string parameter)
    {
        KitComponent? componentToAppend = HtmlEditor.Kit.Components.FirstOrDefault(x => x.UID.Equals(parameter));

        if (componentToAppend == null)
        {
            throw new InvalidQuickActionParameterException("Parameter is not a valid component name.");
        }

        if (string.IsNullOrEmpty(componentToAppend.Template))
        {
            throw new InvalidQuickActionParameterException("Component has no template.");
        }

        if (string.IsNullOrEmpty(componentToAppend.HtmlTagName) || string.IsNullOrEmpty(componentToAppend.HtmlClassName))
        {
            throw new InvalidQuickActionParameterException("Component has no HTML tag name or class name.");       
        }

        HtmlFilterService svc = new();

        string filteredTemplateContent= svc.FilterWith<ComponentTemplateContentFilter>(componentToAppend.Template);
        
        HtmlElementInfo info = new()
        {
            ComponentUid = componentToAppend.UID,
            InternalId = Guid.NewGuid().ToString(),
            TagName = componentToAppend.HtmlTagName,
            CssClass = componentToAppend.HtmlClassName,
            InnerHtml = filteredTemplateContent,
            ParentInternalId = editor.SelectedElement!.InternalId,
            Index = 99
        };

        InsertElementCommand cmd = new(editor)
        {
            SourceElementInfo = info
        };
        editor.CommandManager.ExecuteCommand(cmd);
    }
}