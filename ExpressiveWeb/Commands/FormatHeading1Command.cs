// *********************************************************
// 
// ExpressiveWeb RedoCommand.cs
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

using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Designer.Models;
using ExpressiveWeb.Localization;
using ExpressiveWeb.Modules.EditorView;

namespace ExpressiveWeb.Commands;

public class FormatHeading1Command : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "Format.Heading1";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.FormatHeading1;
        }
    }

    public override void Execute()
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorView))
        {
            editorView!.Editor.ChangeTextTagType(HtmlDefaultTextTagType.Heading1);
            
        }
    }
}

public class FormatHeading2Command : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "Format.Heading2";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.FormatHeading2;
        }
    }

    public override void Execute()
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorView))
        {
            editorView!.Editor.ChangeTextTagType(HtmlDefaultTextTagType.Heading2);
        }
    }
}

public class FormatHeading3Command : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "Format.Heading3";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.FormatHeading3;
        }
    }

    public override void Execute()
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorView))
        {
            editorView!.Editor.ChangeTextTagType(HtmlDefaultTextTagType.Heading3);
        }
    }
}

public class FormatHeading4Command : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "Format.Heading4";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.FormatHeading4;
        }
    }

    public override void Execute()
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorView))
        {
            editorView!.Editor.ChangeTextTagType(HtmlDefaultTextTagType.Heading4);
        }
    }
}

public class FormatHeading5Command : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "Format.Heading5";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.FormatHeading5;
        }
    }

    public override void Execute()
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorView))
        {
            editorView!.Editor.ChangeTextTagType(HtmlDefaultTextTagType.Heading5);
        }
    }
}

public class FormatHeading6Command : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "Format.Heading6";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.FormatHeading6;
        }
    }

    public override void Execute()
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorView))
        {
            editorView!.Editor.ChangeTextTagType(HtmlDefaultTextTagType.Heading6);
        }
    }
}



public class FormatParagraphCommand : ApplicationCommandBase
{
    public override string CommandName
    {
        get
        {
            return "Format.Paragraph";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.FormatParagraph;
        }
    }

    public override void Execute()
    {
        if (AppState.Instance.AppWindow.ApplicationWorkspaceControl.IsCurrentDocumentOfType(out EditorView? editorView))
        {
            editorView!.Editor.ChangeTextTagType(HtmlDefaultTextTagType.Paragraph);
        }
    }
}