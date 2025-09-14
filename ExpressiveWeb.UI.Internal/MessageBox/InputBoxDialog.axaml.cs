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

using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ExpressiveWeb.Presentation.MessageBox;

public partial class InputBoxDialog : Window
{
    public InputBoxDialog()
    {
        InitializeComponent();
        Loaded += OnLoaded;
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

    private void BtnCancel_Click(object? sender, RoutedEventArgs e)
    {
        SelectedButton = MessageBoxResultButton.Cancel;
        Close(true);
    }

    private void BtnOk_Click(object? sender, RoutedEventArgs e)
    {
        Value = TbValue.Text ?? string.Empty;
        SelectedButton = MessageBoxResultButton.Ok;
        Close(true);
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        TbValue.Text = Value;
    }
}