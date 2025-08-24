// *********************************************************
// 
// ExpressiveWeb.UI.Shell MessageBoxData.cs
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

using Avalonia.Controls;
using Avalonia.Media;

namespace ExpressiveWeb.Presentation.MessageBox;

public class MessageBoxData
{

    public string Message
    {
        get;
        set;
    }

    public string Title
    {
        get;
        set;
    }

    public required Window? Owner
    {
        get;
        set;
    }

    public MessageBoxButtons Buttons
    {
        get;
        set;
    }

    public MessageBoxImage Image
    {
        get;
        set;
    } = MessageBoxImage.None;

    public Brush? Icon
    {
        get;
        set;
    }

    public string? CheckBoxText
    {
        get;
        set;
    }

    public bool CheckBoxInitialState
    {
        get;
        set;
    }

    public string? DefaultButtonText
    {
        get;
        set;
    }

    public string? SecondaryButtonText
    {
        get;
        set;
    }
}

public enum MessageBoxImage
{
    Information,
    Warning,
    Error,
    Question,
    None
}