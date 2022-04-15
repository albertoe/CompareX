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
using CompareX.Core.Contracts;

namespace CompareX.Commands;

[Verb(CommandVerbs.CompareVerb, HelpText = "Compares snapshots")]
class CompareCommand : AbstractCommonCommand
{
    [Option("snapshot1", HelpText = "File snapshot to compare against")]
    public string SnapshotFile1 { get; set; } = string.Empty;

    [Option("snapshot2", HelpText = "File snapshot to compare against")]
    public string SnapshotFile2 { get; set; } = string.Empty;

    private void DisplayResults(CompareResults results)
    {
        if (results.NotFound.Count == 0 && results.Updated.Count == 0 && results.NewFiles.Count == 0)
        {
            Console.WriteLine("No changes found");
            return;
        }

        if (results.NotFound.Count > 0)
        {
            Console.WriteLine("=============");
            Console.WriteLine("Deleted files");
            Console.WriteLine("=============");
            foreach (var file in results.NotFound)
                Console.WriteLine(file);
            Console.WriteLine();
        }

        if (results.Updated.Count > 0)
        {
            Console.WriteLine("=============");
            Console.WriteLine("Updated files");
            Console.WriteLine("=============");

            foreach (var file in results.Updated)
                Console.WriteLine(file);
            Console.WriteLine();
        }

        if (results.NewFiles.Count > 0)
        {
            Console.WriteLine("=============");
            Console.WriteLine("  New File   ");
            Console.WriteLine("=============");

            foreach (var file in results.NewFiles)
                Console.WriteLine(file);
        }
    }

    internal override CommandRunResultEnum Run()
    {
#if DEBUG
        DisplayCommonOptions();
#endif
        if (this.SnapshotFile1.Length == 0 && this.SnapshotFile2.Length == 0)
        {
            // Let's look for the latest signatures file on the current directory
            var signatureFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), $"*{Constants.SnaphostFileExtension}");
            if (signatureFiles.Length == 0)
            {
                this.Errors.Add("There are no snapshots to compare to");
                return CommandRunResultEnum.CommandExecutionError;
            }

            Array.Sort(signatureFiles);
            // Get the last version and use it as a default
            this.SnapshotFile1 = signatureFiles[signatureFiles.Length - 1];
            Console.WriteLine($"Using the most current snapshot file {this.SnapshotFile1}");
        }

        // Do your magic
        if (!File.Exists(this.SnapshotFile1))
        {
            this.Errors.Add($"Snapshot '{this.SnapshotFile1}' does not exist");
            return CommandRunResultEnum.InvalidParameter;
        }

        var source1 = JsonStorage.Load(this.SnapshotFile1);
        if (source1 == null)
        {
            this.Errors.Add($"Invalid snapshot file '{this.SnapshotFile1}'");
            return CommandRunResultEnum.InvalidParameter;
        }

        FileSignatures? source2 = null;
        if (this.SnapshotFile2.Length != 0)
        {
            if (!File.Exists(this.SnapshotFile2))
            {
                this.Errors.Add($"Snapshot '{this.SnapshotFile2}' does not exist");
                return CommandRunResultEnum.InvalidParameter;
            }

            source2 = JsonStorage.Load(this.SnapshotFile2);
            if (source2 == null)
            {
                this.Errors.Add($"Invalid snapshot file '{this.SnapshotFile2}'");
                return CommandRunResultEnum.InvalidParameter;
            }
        }
        else
        {
            var engine = new SnapshotEngine(source1.RootFolder, source1.IncludesChildDirs);
            source2 = engine.GetSnapshot();
        }

        var compareEngine = new CompareEngine(source1, source2);
        var results = compareEngine.Compare();

        DisplayResults(results);

        return CommandRunResultEnum.Success;
    }

    internal override string SampleUsageHelp(IEnumerable<Error> errors)
    {
        // Add any samples on how to use your command
        return $"   {Constants.ExeName}.exe {CommandVerbs.CompareVerb} {CommandVerbs.CompareVerbExample}";
    }
}