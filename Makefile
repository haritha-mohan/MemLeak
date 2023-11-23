BUILDDIR = ./MemLeak/bin/Release
.PHONY: all clean

all: run

build:
	dotnet build ./MemLeak/MemLeak.csproj -c Release --property WarningLevel=0

analyze: DUMP := $(value DUMP) NAMESPACE := $(value NAMESPACE)
analyze: build
	$(BUILDDIR)/MemLeak analyze --dump="$(DUMP)" --namespace="$(NAMESPACE)"

help: build
	$(BUILDDIR)/MemLeak --help