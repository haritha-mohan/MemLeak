# MemLeak
A x-plat command line tool that generates a simple visual representation of strongly connected components- which could be an indication of a potential memory leak. 
Especially helpful when application is walking the bridge between managed and unmanaged memory.

## MemLeak, from a functional perspective
- input: path to app (our use case specifically MAUI app)
- process:
  - run app
  - invoke new [mono supported dotnet-gcdump](https://github.com/dotnet/runtime/pull/88634)
  - collect the dump/report
  - use inspo from PerfView/VS/Tarjan's algo to detect strongly connected cycles
- output: from cycle analysis, present objects that are culprits for the cycles
  - initial iteration: just display object, maybe later incorporate via the use of a library, a more comprehensive visual representation
  - thinking about using something like mermaid to diagram the cycle

## Notes from Johan
- dotnet-gcdump (Mono's version) is not part of the latest release
  - will have to build the runtime locally & use this build for tool
  - Mono runtime will handle the same keywords in the EventPipe provider used by the dotnet-gcdump tool and generate the exact same events, so its completely transparent for the gcdump tool, user uses it in the same way on Mono and CoreCLR
- also not compatible yet with mobile apps due to [ds router issue](https://github.com/dotnet/diagnostics/pull/4081), but works fine on desktop processes
  -  so initially will focus on macOS/maccatalyst until the router fix has been merged
- though creating a seprate tool now, this support for detecting strongly connected cycles could eventually be incorporated as an extension to the dotnet-gcdump tool itself rather than having this custom tool separately
  - not getting ahead of myself, need to first implemenet this intiial iteration
  - but could be a neat feature in the future if all goes to plan
- dotnet/diagnostics vs. dotnet/runtime
  - dotnet-* diagnostic tooling is hosted in [dotnet/diagnostics repo](https://github.com/dotnet/diagnostics/tree/fe04962852e7b33864d2272f388f29aa89c92939/src/Tools/dotnet-gcdump)
    - hosts the commands for the tool, etc.
  - runtime implementation of EventPipe and features are implemented in dotnet/runtime 
