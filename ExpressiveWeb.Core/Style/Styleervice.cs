// *********************************************************
// 
// ExpressiveWeb.Core Styleervice.cs
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

using ExCSS;
using ExpressiveWeb.Core.BackgroundServices;
using ExpressiveWeb.Core.Log;

namespace ExpressiveWeb.Core.Style;

public class StyleService : IStyleService
{
    private readonly IBackgroundTaskManager _taskManager;
    private readonly ILogService _logService;

    public StyleService(IBackgroundTaskManager taskManager, ILogService logService)
    {
        _logService = logService;
        _taskManager = taskManager;
    }

    public async Task<Stylesheet> ParseStyleSheet(string styleSheetFilePath)
    {
        using StreamReader streamReader = new StreamReader(styleSheetFilePath);


        StylesheetParser p = new StylesheetParser();
        var styleSheet = await p.ParseAsync(streamReader.BaseStream);


        // foreach (IStyleRule styleRule in styleSheet.StyleRules)
        // {
        //     styleRule.Style
        // }

        // UserStyle s = new();
        // s.RuleSet.Add(new StyleProperty(PropertyNames.FontSize)
        // {
        //     Value = "14px"
        // });

        return styleSheet;
    }
}

public interface IStyleService
{
    Task<Stylesheet> ParseStyleSheet(string styleSheetFilePath);
}