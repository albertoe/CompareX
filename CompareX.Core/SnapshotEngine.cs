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
using Serilog;

namespace CompareX.Core;

public class SnapshotEngine
{
    public SnapshotEngine(string folder, bool processSubDirs)
    {
        this.folder = folder;
        this.processSubDirs = processSubDirs;
    }

    private readonly string folder;
    private readonly bool processSubDirs;

    public FileSignatures GetSnapshot()
    {
        var ret = new FileSignatures(this.folder, this.processSubDirs);

        foreach (var file in Directory.EnumerateFiles(
                     this.folder,
                     "*.*",
                     this.processSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
        {
            // Do not process our own snapshot files
            if (Path.GetExtension(file).Equals(Constants.SnaphostFileExtension, StringComparison.InvariantCultureIgnoreCase))
                continue;

            var fileInfo = new FileInfo(file);
            var signature = FileSignature.GetSignature(fileInfo);
            if (signature == null)
            {
                Log.Warning("Could not calculate signature for 'file'");
                continue;
            }

            // Add the signature
            ret.Signatures.Add(file, signature);
        }

        return ret;
    }

    public void SaveSnapshot()
    {
        var snapshot = GetSnapshot();
        JsonStorage.Save(snapshot);
    }
}