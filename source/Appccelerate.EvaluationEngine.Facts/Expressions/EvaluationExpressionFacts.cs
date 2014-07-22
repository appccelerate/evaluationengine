//-------------------------------------------------------------------------------
// <copyright file="EvaluationExpressionFacts.cs" company="Appccelerate">
//   Copyright (c) 2008-2014
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Appccelerate.EvaluationEngine.Expressions
{
    using System.Reflection;

    using FluentAssertions;

    using Xunit;

    public class EvaluationExpressionFacts
    {
        [Fact]
        public void PassesCallToDerivedClass()
        {
            const string ExpectedResult = "result";
            var testee = new TestableEvaluationExpression(ExpectedResult);

            var result = testee.Evaluate(Missing.Value);

            result
                .Should().Be(
                    ExpectedResult, 
                    "because the call to Evaluate with parameter on base class should be relayed to Evaluate without parameter of derived class.");
        }

        private class TestableEvaluationExpression : EvaluationExpression<string>
        {
            public TestableEvaluationExpression(string result)
            {
                this.Result = result;
            }

            private string Result { get; set; }

            protected override string Evaluate()
            {
                return this.Result;
            }
        }
    }
}