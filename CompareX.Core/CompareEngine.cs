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

using CompareX.Core.Contracts;

namespace CompareX.Core;

public class CompareEngine
{
    public CompareEngine(FileSignatures signatures1, FileSignatures signatures2)
    {
        this.original = signatures1;
        this.compare = signatures2;
    }

    private readonly FileSignatures compare;

    private readonly FileSignatures original;

    public CompareResults Compare()
    {
        var results = new CompareResults();
        foreach (var source in this.original.Signatures)
        {
            var sourceFile = source.Key;
            var sourceSignature = source.Value;
            if (!this.compare.Signatures.ContainsKey(sourceFile))
            {
                results.NotFound.Add(sourceFile);
                continue;
            }

            // File names match so let's check other details
            var compareTo = this.compare.Signatures[sourceFile];
            compareTo.HasBeenCompared = true;
            if (sourceSignature.Signature != compareTo.Signature)
                results.Updated.Add(sourceFile);

        }

        // Loop through all the destination files and add any new files contained in it
        foreach (var item in this.compare.Signatures)
            if (!item.Value.HasBeenCompared)

                results.NewFiles.Add(item.Key);

        return results;
    }
}