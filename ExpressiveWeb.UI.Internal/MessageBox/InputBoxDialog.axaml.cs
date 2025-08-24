// *********************************************************
// 
// ExpressiveWeb.UI.Internal InputBoxDialog.axaml.cs
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
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ExpressiveWeb.Presentation.MessageBox;

public partial class InputBoxDialog : Window
{
    public InputBoxDialog()
    {
        InitializeComponent();
        Loaded+= OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        this.TbValue.Text = Value;
    }

    internal MessageBoxResultButton SelectedButton
    {
        get;
        private set;
    }

    internal string Value
    {
        get;
        set;
    }

    private void BtnOk_Click(object? sender, RoutedEventArgs e)
    {
        Value = this.TbValue.Text;
        SelectedButton = MessageBoxResultButton.Ok;
        Close(true);
    }

    private void BtnCancel_Click(object? sender, RoutedEventArgs e)
    {
        SelectedButton = MessageBoxResultButton.Cancel;
        Close(true);
    }
}