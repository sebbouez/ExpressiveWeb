// *********************************************************
// 
// ExpressiveWeb ExitApplicationCommand.cs
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

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Localization;

namespace ExpressiveWeb.Commands;

public class ExitApplicationCommand : ApplicationCommandBase
{
    public ExitApplicationCommand()
    {
        IsEnabled = true;
    }

    public override string CommandName
    {
        get
        {
            return "ExitApplication";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.ExitApplication;
        }
    }

    public override void Execute()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}