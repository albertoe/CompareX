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

using System;
using System.IO;
using CompareX.Core.Contracts;
using FluentAssertions;
using Newtonsoft.Json;

namespace CompareX.Tests.Helpers
{
    /// <summary>
    ///     Class to create fake signatures
    /// </summary>
    internal static class FileSignaturesFakes
    {
        internal static FileSignature AddFakeSignature(
            this FileSignatures signatures,
            string fileName,
            string? signature = null,
            DateTime? fileDate = null,
            long fileSize = 0)
        {
            //  If null use defaults
            signature ??= "TestSignature";
            fileDate ??= new DateTime(2001, 1, 1, 1, 1, 1);
            if (fileSize == 0)
                fileSize = 12345678;

            var fileSignature = new FileSignature {Signature = signature, FileDate = (DateTime)fileDate, FileSize = fileSize};

            signatures.Signatures.Add(fileName, fileSignature);
            return fileSignature;
        }

        internal static FileSignatures Clone(this FileSignatures @this)
        {
            var json = JsonConvert.SerializeObject(@this);
            return JsonConvert.DeserializeObject<FileSignatures>(json)!;
        }

        internal static FileSignatures GetSnapshotFromFIle(string snapshot1File)
        {
            var fileName = Path.Combine(Globals.TestFilesFolder, snapshot1File);
            var json = File.ReadAllText(fileName);
            var fileSignatures = JsonConvert.DeserializeObject<FileSignatures>(json);
            fileSignatures.Should().NotBeNull();
            return fileSignatures!;
        }
    }
}