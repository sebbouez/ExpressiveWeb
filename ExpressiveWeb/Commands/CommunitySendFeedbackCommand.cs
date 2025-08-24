// *********************************************************
// 
// ExpressiveWeb CommunitySendFeedbackCommand.cs
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

using System.Diagnostics;
using ExpressiveWeb.Core;
using ExpressiveWeb.Core.ApplicationCommands;
using ExpressiveWeb.Localization;

namespace ExpressiveWeb.Commands;

public class CommunitySendFeedbackCommand : ApplicationCommandBase
{
    public CommunitySendFeedbackCommand()
    {
        IsEnabled = true;
    }

    public override string CommandName
    {
        get
        {
            return "Community.SendFeedback";
        }
    }

    public override string Title
    {
        get
        {
            return Resources.CommunitySendFeedback;
        }
    }

    public override void Execute()
    {
        ProcessStartInfo psi = new()
        {
            FileName = UrlConstants.URL_GITHUB_BUGS,
            UseShellExecute = true
        };
        _ = Process.Start(psi);
    }
}