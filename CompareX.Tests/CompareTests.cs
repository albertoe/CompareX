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

using System.Collections.Generic;
using CompareX.Core;
using CompareX.Core.Contracts;
using CompareX.Tests.Helpers;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CompareX.Tests;

public class CompareTests
{
    public CompareTests(ITestOutputHelper output)
    {
        TestUtils.SetOutputToSerilog(output);
    }

    private const string Snapshot1File = "Test1File.json";

    [Fact]
    public void Added1File()
    {
        var original = new FileSignatures();
        original.AddFakeSignature("File1");
        var current = original.Clone();
        current.AddFakeSignature("File2");

        var results = new CompareEngine(original, current).Compare();

        results.NotFound.Count.Should().Be(0);
        results.NewFiles.Count.Should().Be(1);
        results.Updated.Count.Should().Be(0);
    }

    [Fact]
    public void Deleted1File()
    {
        var original = new FileSignatures();
        original.AddFakeSignature("File1");
        var current = original.Clone();
        current.Signatures = new Dictionary<string, FileSignature>();

        var results = new CompareEngine(original, current).Compare();

        results.NotFound.Count.Should().Be(1);
        results.NewFiles.Count.Should().Be(0);
        results.Updated.Count.Should().Be(0);
    }

    [Fact]
    public void IdenticalResults()
    {
        var source = FileSignaturesFakes.GetSnapshotFromFIle(Snapshot1File);

        var results = new CompareEngine(source, source).Compare();

        results.NotFound.Count.Should().Be(0);
        results.NewFiles.Count.Should().Be(0);
        results.Updated.Count.Should().Be(0);
    }

    [Fact]
    public void UpdatedFile()
    {
        var original = new FileSignatures();
        original.AddFakeSignature("File1");
        var current = original.Clone();
        current.Signatures["File1"].Signature = "NewSignature";

        var results = new CompareEngine(original, current).Compare();

        results.NotFound.Count.Should().Be(0);
        results.NewFiles.Count.Should().Be(0);
        results.Updated.Count.Should().Be(1);
    }
}