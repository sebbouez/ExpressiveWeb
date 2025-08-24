// *********************************************************
// 
// ExpressiveWeb.Core ApplicationCommandBase.cs
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

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExpressiveWeb.Core.ApplicationCommands;

public abstract class ApplicationCommandBase : INotifyPropertyChanged
{
    private bool _isChecked;
    private bool _isEnabled;

    public bool IsEnabled
    {
        get
        {
            return _isEnabled;
        }
        set
        {
            SetField(ref _isEnabled, value);
        }
    }

    public bool IsChecked
    {
        get
        {
            return _isChecked;
        }
        set
        {
            SetField(ref _isChecked, value);
        }
    }

    public virtual List<ApplicationCommandBase>? SubCommands
    {
        get;
    } = null;

    public bool HasSubCommands
    {
        get
        {
            return SubCommands != null && SubCommands.Count > 0;
        }
    }

    public virtual string Description
    {
        get
        {
            return string.Empty;
        }
    }

    public abstract string CommandName
    {
        get;
    }

    public abstract string Title
    {
        get;
    }

    public virtual string IconResourceName
    {
        get;
    } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;
    public abstract void Execute();

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}