// *********************************************************
// 
// ExpressiveWeb AboutApplicationCommand.cs
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
using ExpressiveWeb.Modules.Common;

namespace ExpressiveWeb.Commands;

public class AboutApplicationCommand : ApplicationCommandBase
{
    public AboutApplicationCommand()
    {
        IsEnabled = true;       
    }

    public override string CommandName
    {
        get
        {
            return "AboutApplication";
        }
    }

    public override string Title
    {
        get
        {
            return Localization.Resources.MenuAbout;
        }
    }

    public override void Execute()
    {
        AboutDialog dlg = new();
        dlg.ShowDialog(AppState.Instance.AppWindow);
    }
}