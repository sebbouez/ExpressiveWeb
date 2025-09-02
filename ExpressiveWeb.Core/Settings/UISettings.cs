// *********************************************************
// 
// ExpressiveWeb.Core UISettings.cs
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

namespace ExpressiveWeb.Core.Settings;

public class UISettings
{
    public List<string> MainToolbarLeftCommands
    {
        get;
        set;
    } = new();

    public List<string> MainToolbarRightCommands
    {
        get;
        set;
    }= new();

    public List<string> MainToolbarCenterCommands
    {
        get;
        set;
    }= new();
    
    public WindowSizeOptions MainWindowSize
    {
        get;
        set;
    } = new();
}

public class WindowSizeOptions
{
    public int? WindowWidth { get; set; }
    public int? WindowHeight { get; set; }
    public int? WindowX { get; set; }
    public int? WindowY { get; set; }
    public bool IsMaximized { get; set; }
}