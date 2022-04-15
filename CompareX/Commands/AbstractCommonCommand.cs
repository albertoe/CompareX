// Copyright 2022 CompareX Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using CommandLine;
using CompareX.Core;
using Serilog.Events;

namespace CompareX.Commands;

enum CommandRunResultEnum
{
    Success = 0,
    CommandExecutionError = 1,
    InvalidParameter = 10
}

/// <summary>
///     This class defines the options that can be used by all commands
/// </summary>
abstract class AbstractCommonCommand
{
    internal List<string> Errors = new();

    [Option("loglevel", Default = 2, Hidden = true, HelpText = "Logging level 0=Error, 1=Warning, 2=Information 3=Debug (Default=2)")]
    public int LogLevel { get; set; }

    [Option("LogToSeq", Default = false, Hidden = true, HelpText = "Prints all messages to standard output.")]
    public bool LogToSeq { get; set; }

    // Omitting long name, defaults to name of property, ie "--verbose"
    [Option('v', "Verbose", Default = false, Hidden = true, HelpText = "Prints all messages to standard output.")]
    public bool Verbose { get; set; }

    internal void DisplayCommonOptions()
    {
        Console.WriteLine($"Verbose = {this.Verbose}");
        Console.WriteLine($"LogLevel = {this.LogLevel}");
    }

    internal void Initialize()
    {
        switch (this.LogLevel)
        {
            case 1:
                Config.LogLevel = LogEventLevel.Warning;
                break;
            case 2:
                Config.LogLevel = LogEventLevel.Information;
                break;
            case 3:
                Config.LogLevel = LogEventLevel.Debug;
                break;
            case 4:
                Config.LogLevel = LogEventLevel.Verbose;
                break;
            default:
                Config.LogLevel = LogEventLevel.Information;
                break;
        }
    }

    internal void ReportError(string error)
    {
        this.Errors.Add(error);
    }

    internal abstract CommandRunResultEnum Run();
    internal abstract string SampleUsageHelp(IEnumerable<Error> errors);
}