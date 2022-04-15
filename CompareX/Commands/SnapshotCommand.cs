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

namespace CompareX.Commands;

[Verb(CommandVerbs.SnapshotVerb, HelpText = "Creates a snapshot of the directory structure")]
class SnapshotCommand : AbstractCommonCommand
{
    [Option('s', HelpText = "Process child directories")]
    public bool ProcessChildren { get; set; }

    [Option("source", HelpText = "Directory to take snapshot from (Default to current dir)")]
    public string SourceDirectory { get; set; } = string.Empty;

    internal override CommandRunResultEnum Run()
    {
#if DEBUG
        DisplayCommonOptions();
#endif
        // Do your magic
        // Default to current Directory
        if (this.SourceDirectory.Length == 0)
            this.SourceDirectory = Directory.GetCurrentDirectory();

        if (!Directory.Exists(this.SourceDirectory))
        {
            this.Errors.Add($"Folder '{this.SourceDirectory}' does not exist");
            return CommandRunResultEnum.InvalidParameter;
        }

        var engine = new SnapshotEngine(this.SourceDirectory, this.ProcessChildren);
        engine.SaveSnapshot();

        return CommandRunResultEnum.Success;
    }

    internal override string SampleUsageHelp(IEnumerable<Error> errors)
    {
        // Add any samples on how to use your command
        return $"   {Constants.ExeName}.exe {CommandVerbs.SnapshotVerb} {CommandVerbs.SnapshotVerbExample}";
    }
}