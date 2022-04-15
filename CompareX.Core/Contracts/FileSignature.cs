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

using Newtonsoft.Json;

namespace CompareX.Core.Contracts;

public class FileSignature
{
    [JsonConstructor]
    public FileSignature()
    {
    }

    [JsonIgnore]
    internal bool HasBeenCompared;

    [JsonProperty("ModifiedDate")]
    public DateTime FileDate { get; set; }

    [JsonProperty("Size")]
    public long FileSize { get; set; }

    [JsonProperty("Signature")]
    public string Signature { get; set; } = string.Empty;

    private static string GetFileSignature(IFileSignatureProvider provider, FileInfo file)
    {
        return provider.GetSignature(file);
    }

    internal static FileSignature? GetSignature(FileInfo file)
    {
        if (!file.Exists)
            return null;

        return new FileSignature
               {
                   FileSize = file.Length, FileDate = file.LastWriteTimeUtc, Signature = GetFileSignature(new SignatureProviderMD5(), file)
               };
    }
}