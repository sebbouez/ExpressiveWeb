// *********************************************************
// 
// ExpressiveWeb.UI.Internal EWUnitNumericTextBox.cs
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

namespace ExpressiveWeb.Presentation.TextBox;

public class EWUnitNumericTextBox : NumericUpDown
{
    public static readonly StyledProperty<string> UnitTextProperty =
        AvaloniaProperty.Register<EWUnitNumericTextBox, string>(nameof(UnitText), string.Empty);
    
    
    public string UnitText
    {
        get
        {
            return GetValue(UnitTextProperty);
        }
        set
        {
            SetValue(UnitTextProperty, value);
        }
    }
}