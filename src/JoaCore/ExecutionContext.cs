﻿using JoaPluginsPackage;

namespace JoaCore;

public class ExecutionContext : IExecutionContext
{
    public ContextAction ContextAction { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
}