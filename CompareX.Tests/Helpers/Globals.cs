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
using System.Diagnostics;
using System.IO;

namespace CompareX.Tests.Helpers;

internal class Globals
{
    static Globals()
    {

        TestRunnerIsInDebugMode = Debugger.IsAttached;
        Debug.Assert(
            AppDomain.CurrentDomain.SetupInformation.ApplicationBase != null,
            "AppDomain.CurrentDomain.SetupInformation.ApplicationBase != null");
        testBaseFolder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        TestFilesFolder = Path.Combine(testBaseFolder!, "TestFiles");
    }

    private static readonly string testBaseFolder;
    public static readonly string TestFilesFolder;

    public static bool TestRunnerIsInDebugMode { get; }
}