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

namespace CompareX.Commands;

internal static class CommandVerbs
{
    internal const string CompareVerb = "compare";
    internal const string CompareVerbExample = @"--param1 asdf --param2";
    internal const string SnapshotVerb = "snapshot";
    internal const string SnapshotVerbExample = @"--source c:\MyDir -s";
}