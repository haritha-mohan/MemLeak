using Mono.Options;

namespace MemLeak.Commands;

internal abstract class MLCommand : Command
{
    protected bool ShowHelp = false;
    public MLCommand(string name, string help = null) : base(name, help) { }
    protected MLCommand(string name) : this(name, string.Empty) { }

    public sealed override int Invoke(IEnumerable<string> arguments)
    {
        Options.Parse(arguments);
        if (ShowHelp)
        {
            Options.WriteOptionDescriptions(Console.Out);
            return 0;
        }

        return InvokeInternal();
    }

    protected abstract int InvokeInternal();
}