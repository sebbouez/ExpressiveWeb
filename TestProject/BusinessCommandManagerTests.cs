using ExpressiveWeb.Core;
using ExpressiveWeb.Core.Commands;
using ExpressiveWeb.Core.Log;
using Microsoft.Extensions.DependencyInjection;

namespace TestProject;

public class BusinessCommandManagerTests
{
    private sealed class DummyLogService : ILogService
    {
        public void Info(string message) { }
        public void Warning(string message) { }
        public void Error(string message) { }
        public void Error(Exception ex) { }
        public void Dispose() { }
    }

    private sealed class FakeCommand : IBusinessCommand
    {
        public int DoCalls { get; private set; }
        public int UndoCalls { get; private set; }
        public readonly List<string> Trace = new();
        private readonly string _name;
        public FakeCommand(string name = "cmd") => _name = name;
        public void Do()
        {
            DoCalls++;
            Trace.Add($"{_name}:Do");
        }
        public void Undo()
        {
            UndoCalls++;
            Trace.Add($"{_name}:Undo");
        }
    }

    private static void EnsureServices()
    {
        // Create DI container with ILogService so BusinessCommandManager can resolve it
        ServiceCollection sc = new();
        sc.AddSingleton<ILogService, DummyLogService>();
        AppServices.ServicesFactory = sc.BuildServiceProvider();
    }

    [Fact]
    public void Execute_Undo_Redo_ShouldManageStacksAndInvokeCommands()
    {
        EnsureServices();
        BusinessCommandManager sut = new();
        bool? lastChangedArg = null;
        int changedCount = 0;
        sut.Changed += (_, arg) => { lastChangedArg = arg; changedCount++; };

        var cmd = new FakeCommand("A");

        // Execute
        sut.ExecuteCommand(cmd);
        Assert.Equal(1, cmd.DoCalls);
        Assert.True(sut.CanUndo);
        Assert.False(sut.CanRedo);
        Assert.True(sut.HasCommands);
        Assert.Equal(false, lastChangedArg);
        Assert.True(changedCount >= 1);

        // Undo
        sut.Undo();
        Assert.Equal(1, cmd.UndoCalls);
        Assert.True(sut.CanRedo);
        Assert.False(sut.CanUndo);
        Assert.Equal(true, lastChangedArg);

        // Redo
        sut.Redo();
        Assert.Equal(2, cmd.DoCalls);
        Assert.True(sut.CanUndo);
        Assert.False(sut.CanRedo);
        Assert.Equal(true, lastChangedArg);

        // Reset
        sut.Reset();
        Assert.False(sut.CanUndo);
        Assert.False(sut.CanRedo);
        Assert.False(sut.HasCommands);
        Assert.Equal(false, lastChangedArg);
    }

    [Fact]
    public void ExecuteCommandSilent_ShouldPushWithoutExecuting()
    {
        EnsureServices();
        BusinessCommandManager sut = new();
        var cmd = new FakeCommand();

        sut.ExecuteCommandSilent(cmd);

        Assert.Equal(0, cmd.DoCalls);
        Assert.True(sut.CanUndo);
        Assert.False(sut.CanRedo);

        sut.Undo();
        Assert.Equal(1, cmd.UndoCalls);
    }

    [Fact]
    public void IsReadonly_ShouldPreventMutations()
    {
        EnsureServices();
        BusinessCommandManager sut = new() { IsReadonly = true };
        var cmd = new FakeCommand();

        sut.ExecuteCommand(cmd);
        sut.ExecuteCommandSilent(cmd);
        sut.Undo();
        sut.Redo();
        sut.Reset();

        Assert.Equal(0, cmd.DoCalls);
        Assert.Equal(0, cmd.UndoCalls);
        Assert.False(sut.CanUndo);
        Assert.False(sut.CanRedo);
        Assert.False(sut.HasCommands);
    }

    [Fact]
    public void OpenAndCloseContext_ShouldChangeContextAndMaintainStacks()
    {
        EnsureServices();
        BusinessCommandManager sut = new();
        Assert.Equal(string.Empty, sut.CurrentContext);

        // Open a context and execute there
        sut.OpenContext("ctx1");
        Assert.Equal("ctx1", sut.CurrentContext);
        var cmd1 = new FakeCommand("C1");
        sut.ExecuteCommand(cmd1);
        Assert.True(sut.CanUndo);

        // Open nested/new context
        sut.OpenContext("ctx2");
        Assert.Equal("ctx2", sut.CurrentContext);
        var cmd2 = new FakeCommand("C2");
        sut.ExecuteCommand(cmd2);
        Assert.True(sut.CanUndo);

        // Close ctx2, should revert to ctx1
        sut.CloseContext("ctx2");
        Assert.Equal("ctx1", sut.CurrentContext);
        // Undo should now refer to ctx1 stack
        sut.Undo();
        Assert.Equal(1, cmd1.UndoCalls);

        // Close ctx1, should revert to root
        sut.CloseContext("ctx1");
        Assert.Equal(string.Empty, sut.CurrentContext);
    }
}
