# MemLeak
A x-plat command line tool that generates a simple visual representation of strongly connected components- which could be an indication of a potential memory leak. 
Especially helpful when application is walking the bridge between managed and unmanaged memory.

## How to use MemLeak
sample command: analyze -dump="20230729_134428_16855.gcdump"

## From a functional perspective
- input: path to app (our use case specifically MAUI app)
- process:
  - run app
  - invoke new [mono supported dotnet-gcdump](https://github.com/dotnet/runtime/pull/88634)
  - collect the dump/report
  - use inspo from PerfView/VS/Tarjan's algo to detect strongly connected cycles
- output: from cycle analysis, present objects that are culprits for the cycles
  - initial iteration: just display details of cycle
  - [wip] using something like graphviz or a console library to diagram the cycle
