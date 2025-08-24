// *********************************************************
// 
// ExpressiveWeb.UI.Internal EWInputBox.cs
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

namespace ExpressiveWeb.Presentation.MessageBox;

public static class EWInputBox
{
    public static async Task<string?> Show(InputBoxData data)
    {
        InputBoxDialog dlg = new();

        dlg.Title = "";
        dlg.TbTitle.Text = data.Title;
        dlg.TbMessage.Text = data.Message;
        bool result = await dlg.ShowDialog<bool>(data.Owner);

        if (!result)
        {
            return null;
        }

        return dlg.Value;
    }
}