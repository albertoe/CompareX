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
using Newtonsoft.Json;

namespace CompareX.Core;

public class JsonStorage : ISignatureStorage
{
    public static FileSignatures? Load(string file)
    {
        var json = File.ReadAllText(file);
        return JsonConvert.DeserializeObject<FileSignatures>(json);
    }

    internal static void Save(FileSignatures signatures)
    {
        var json = JsonConvert.SerializeObject(signatures);
        var outputFile = signatures.SnapshotFile();
        File.WriteAllText(outputFile, json);
    }
}