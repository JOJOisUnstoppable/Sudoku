﻿namespace Sudoku.Platforms.QQ.Operation;

/// <summary>
/// Defines a periodic operation.
/// </summary>
/// <param name="TriggeringTime">
/// Indicates the <see cref="TimeOnly"/> instance that describes the time that the operation will be triggered daily.
/// </param>
public abstract record PeriodicOperation(TimeOnly TriggeringTime) : OperationBase;
