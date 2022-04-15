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

using System.Diagnostics;
using System.Reflection;
using System.Text;
using CommandLine;
using CommandLine.Text;
using CompareX.Commands;
using CompareX.Core;
using Serilog;

namespace CompareX;

static class Program
{
    static CommandRunResultEnum commandRunResult = CommandRunResultEnum.Success;

    static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors)
    {
        var sampleUsage = new StringBuilder();

        // Look into the errors and create appropriate help info
        if (result.TypeInfo.Current.FullName!.Contains(".Commands.") && result.TypeInfo.Current.FullName!.Contains("CommandVerbs"))
        {
            if (Activator.CreateInstance(result.TypeInfo.Current) is AbstractCommonCommand commandAffected)
            {
                sampleUsage.AppendLine("Usage Example(s):");
                sampleUsage.AppendLine(commandAffected.SampleUsageHelp(errors));
            }
        }
        else
        {
            sampleUsage.AppendLine("Command Help:");
            sampleUsage.AppendLine($"   {Constants.ExeName}.exe [verb] --help");
            sampleUsage.AppendLine();
            sampleUsage.AppendLine("Generic syntax:");
            sampleUsage.AppendLine($"   {Constants.ExeName}.exe [verb] [verb specific options]");
        }

        var helpText = HelpText.AutoBuild(
            result,
            h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = $"{Constants.AppName} Version {Constants.Version})";
                h.Copyright = $"{Constants.Copyright}";
                h.MaximumDisplayWidth = 79;
                h.AddPostOptionsLine(sampleUsage.ToString());
                return HelpText.DefaultParsingErrorsHandler(result, h);
            },
            e => e);
        commandRunResult = CommandRunResultEnum.InvalidParameter;
        Console.WriteLine(helpText);
    }

    static int Main(string[] args)
    {
#if DEBUG
        var started = DateTime.Now;
#endif
        Config.ConsoleLogger();

        // TODO: Need to review how to handle this properly, because removing this line does not trigger the App Constructor
        //App.RunningInteractively = Environment.UserInteractive;

#if RELEASE
            // Prevent the user for using CTRL-C to terminate the process
            Console.TreatControlCAsInput = true;
#endif

        // Load the valid commands that can be executed by getting a list of all of the
        // classes that are tagged with the VerbAttribute
        // For info on how to use the library that we use to parse the commands look at
        // https://github.com/commandlineparser/commandline/wiki
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
        // Parse the arguments and run the command
        var parser = new Parser(with => with.HelpWriter = null);
        var parserResult = parser.ParseArguments(args, types);
        parserResult.WithParsed(RunSelectedCommand).WithNotParsed(errs => DisplayHelp(parserResult, errs));

        Log.Information($"{Constants.AppName} finishing returning {commandRunResult}");

        // Make sure that we write any log entries before we exit
        Log.CloseAndFlush();

#if DEBUG
        Console.WriteLine();
        Console.WriteLine($"Started: {started} Ended : {DateTime.Now}");
        Console.WriteLine();
        Console.WriteLine($"Returning {(int)commandRunResult}");
#endif

        return (int)commandRunResult;
    }

    static void RunSelectedCommand(object obj)
    {
        var command = obj as AbstractCommonCommand;
        if (command == null)
        {
            // If you get here make sure that the command inherits from AbstractCommonCommand
            if (Debugger.IsAttached)
                Debugger.Break();
            Console.WriteLine("Internal Error: Contact Technical support.");
            commandRunResult = CommandRunResultEnum.CommandExecutionError;
            return;
        }

        command.Initialize();
        commandRunResult = command.Run();

        // Return if we did not get any errors
        if (!command.Errors.Any())
            return;

        // Write the errors and return a CommandExecutionError
        Console.WriteLine("");
        Console.WriteLine(command.Errors.Count == 1 ? "** Error **" : "** Errors **");
        Console.WriteLine("");
        foreach (var error in command.Errors)
            Console.WriteLine(error);
        commandRunResult = CommandRunResultEnum.CommandExecutionError;
    }
}