﻿using MemLeak.Commands.AnalyzeCommandSet;
using Mono.Options;

namespace MemLeak;
public static class Program
{
    public static int Main(string[] args)
    {
        CommandSet commands = new("memleak")
        {
            new AnalyzeCommand(),
        };
        return commands.Run(args);
    }
}