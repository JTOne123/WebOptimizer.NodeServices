# NodeServices base class for LigerShark.WebOptimizer

[![Build status](https://ci.appveyor.com/api/projects/status/lc6tvkgisiesw6vh?svg=true)](https://ci.appveyor.com/project/madskristensen/weboptimizer-nodeservices)
[![NuGet](https://img.shields.io/nuget/v/LigerShark.WebOptimizer.NodeServices.svg)](https://nuget.org/packages/LigerShark.WebOptimizer.NodeServices/)

This package compiles TypeScript, ES6 and JSX files into ES5 by hooking into the [LigerShark.WebOptimizer](https://github.com/ligershark/WebOptimizer) pipeline.

## Usage

When creating a custom `IProcessor` using node.js, then use the `NodeProcessor` base class in this assembly.