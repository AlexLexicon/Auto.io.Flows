﻿namespace Auto.io.Flows.Application.Models;
public interface IRunner
{
    public Task PauseAsync();
    public bool IsPaused { get; }
}
