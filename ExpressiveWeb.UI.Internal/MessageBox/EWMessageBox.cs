// *********************************************************
// 
// ExpressiveWeb.UI.Shell EWMessageBox.cs
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

public static class EWMessageBox
{
    public static async Task<MessageBoxResult> Show(MessageBoxData data)
    {
        MessageBoxResult result = new();

        MessageBoxDialog dlg = new();

        switch (data.Buttons)
        {
            case MessageBoxButtons.Ok:
                dlg.BtnOk.IsVisible = true;
                dlg.BtnCancel.IsVisible = false;
                dlg.BtnYes.IsVisible = false;
                dlg.BtnNo.IsVisible = false;
                break;

            case MessageBoxButtons.OkCancel:
                dlg.BtnOk.IsVisible = true;
                dlg.BtnCancel.IsVisible = true;
                dlg.BtnYes.IsVisible = false;
                dlg.BtnNo.IsVisible = false;
                break;

            case MessageBoxButtons.YesNoCancel:
                dlg.BtnOk.IsVisible = false;
                dlg.BtnCancel.IsVisible = true;
                dlg.BtnYes.IsVisible = true;
                dlg.BtnNo.IsVisible = true;

                if (!string.IsNullOrEmpty(data.DefaultButtonText))
                {
                    dlg.BtnYes.Content = data.DefaultButtonText;
                    //dlg.BtnYes.Style = (Style) dlg.FindResource("StdButtonAccent");
                }

                if (!string.IsNullOrEmpty(data.SecondaryButtonText))
                {
                    dlg.BtnNo.Content = data.SecondaryButtonText;
                }

                break;

            case MessageBoxButtons.YesNo:
                dlg.BtnOk.IsVisible = false;
                dlg.BtnCancel.IsVisible = false;
                dlg.BtnYes.IsVisible = true;
                dlg.BtnNo.IsVisible = true;

                if (!string.IsNullOrEmpty(data.DefaultButtonText))
                {
                    dlg.BtnYes.Content = data.DefaultButtonText;
                    //dlg.BtnYes.Style = (Style) dlg.FindResource("StdButtonAccent");
                }

                if (!string.IsNullOrEmpty(data.SecondaryButtonText))
                {
                    dlg.BtnNo.Content = data.SecondaryButtonText;
                }

                break;
        }

        switch (data.Image)
        {
            case MessageBoxImage.Error:
                dlg.RectDialogImage.IsVisible = true;
                dlg.RectDialogImage.Fill = (IBrush?) dlg.FindResource("DialogIconError");
                break;

            case MessageBoxImage.Information:
                dlg.RectDialogImage.IsVisible = true;
                dlg.RectDialogImage.Fill = (IBrush?) dlg.FindResource("DialogIconInformation");
                break;

            case MessageBoxImage.Question:
                dlg.RectDialogImage.IsVisible = true;
                dlg.RectDialogImage.Fill = (IBrush?) dlg.FindResource("DialogIconQuestion");
                break;

            default:
                dlg.RectDialogImage.IsVisible = false;
                break;
        }

        if (data.Icon != null)
        {
            dlg.RectDialogImage.IsVisible = true;
            dlg.RectDialogImage.Fill = data.Icon;
        }

        dlg.Title = "";
        dlg.TbTitle.Text = data.Title;
        dlg.TbMessage.Text = data.Message;
        dlg.CbExtra.IsVisible = !string.IsNullOrEmpty(data.CheckBoxText);
        dlg.CbExtra.Content = data.CheckBoxText;
        await dlg.ShowDialog(data.Owner);

        result.SelectedButton = dlg.SelectedButton;

        if (dlg.CbExtra.IsVisible)
        {
            result.State = dlg.CbExtra.IsChecked;
        }

        return result;
    }
}