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

using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;

namespace CompareX.Core;

public static class Config
{
    static Config()
    {
        logSeqUrl = "http://localhost:5341";
        logLevelSwitch.MinimumLevel = LogEventLevel.Warning;
    }

    static readonly LoggingLevelSwitch logLevelSwitch = new();
    static string logSeqUrl;
    static LogTypeEnum LogType = LogTypeEnum.DebugConsole;

    public static LogEventLevel LogLevel
    {
        get => logLevelSwitch.MinimumLevel;
        set => logLevelSwitch.MinimumLevel = value;
    }

    public static void ConsoleLogger()
    {
        LogType = LogTypeEnum.Console;
        ReconfigureLogger();
    }

    static void ReconfigureLogger()
    {
        // Adding a template to log extra information
        var template = new StringBuilder();
        template.Append("[{Timestamp:HH:mm:ss} {Level}]");
        template.Append(" [{ThreadId}]");
        template.Append(" {SourceContext} {Message}");
        //template.Append(" [{MemberName}]:{LineNumber}");
        //template.Append(" in method {MemberName} at {FilePath}:{LineNumber}");
        template.Append(" {NewLine}{Exception}");

        // var template = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";
        //var template = "[{Timestamp:HH:mm:ss} {Level}] [{ThreadId}] {SourceContext}{Message} [{MemberName}] at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";
        //var template = "[{Timestamp:HH:mm:ss} {Level}] [{ThreadId}] {SourceContext}{Message} [{MemberName}]:{LineNumber}{NewLine}{Exception}{NewLine}";

        switch (LogType)
        {
            case LogTypeEnum.Console:
                Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(logLevelSwitch).Enrich.WithExceptionDetails().Enrich.WithThreadId()
                                                      .WriteTo.Console(outputTemplate: template.ToString()).CreateLogger();
                break;

            case LogTypeEnum.File:
                // System logs will be located in the Logs folder located inside the InstanceRegistry Folder
                var logsFolder = Path.Combine(Path.GetTempPath(), "loki");
                Directory.CreateDirectory(logsFolder);
                var logName = Path.Combine(logsFolder, "loki.log");

                Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(logLevelSwitch).Enrich.WithExceptionDetails().Enrich.WithThreadId()
                                                      .WriteTo.File(
                                                          logName,
                                                          rollingInterval: RollingInterval.Day,
                                                          outputTemplate: template.ToString()).CreateLogger();
                break;
            case LogTypeEnum.DebugConsole:
                break;

        }

    }

    enum LogTypeEnum
    {
        Console,
        File,
        DebugConsole
    }
}