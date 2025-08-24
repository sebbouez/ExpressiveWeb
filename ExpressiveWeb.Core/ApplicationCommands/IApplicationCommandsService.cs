// *********************************************************
// 
// ExpressiveWeb.Core IApplicationCommandsService.cs
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

namespace ExpressiveWeb.Core.ApplicationCommands;

public interface IApplicationCommandsService
{
    List<ApplicationCommandBase> RegisteredCommands
    {
        get;
    }

    T? GetCommand<T>() where T : ApplicationCommandBase;

    void RegisterCommands(List<ApplicationCommandBase> commands);

    void SetCommandState<T>(bool state);
    void RegisterCommand(ApplicationCommandBase command);
}