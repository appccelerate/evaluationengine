//-------------------------------------------------------------------------------
// <copyright file="EvaluationEngineModuleFacts.cs" company="Appccelerate">
//   Copyright (c) 2008-2013
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

namespace Appccelerate.EvaluationEngine
{
    using FakeItEasy;

    using Xunit;

    public class EvaluationEngineModuleFacts
    {
        private readonly TestableModule testee;

        public EvaluationEngineModuleFacts()
        {
            this.testee = new TestableModule();
        }

        [Fact]
        public void AllowsDefinitionOfSolveStrategyForQuestionsWithoutParameter()
        {
            var definition = A.Fake<ISolutionDefinitionHost>();

            this.testee.Load(definition);

            A.CallTo(() => definition.Solve<TestQuestion, string>()).MustHaveHappened();
        }

        [Fact]
        public void AllowsDefinitionOfSolveStrategyForQuestionsWithParameter()
        {
            var definition = A.Fake<ISolutionDefinitionHost>();

            this.testee.Load(definition);

            A.CallTo(() => definition.Solve<TestQuestionWithParameter, string, int>()).MustHaveHappened();
        }

        [Fact]
        public void AllowsDefinitionOfSolveStrategyForQuestionsWithoutParameterAndResultMapping()
        {
            var definition = A.Fake<ISolutionDefinitionHost>();

            this.testee.Load(definition);

            A.CallTo(() => definition.SolveWithResultMapping<TestQuestionWithResultMapping, string, char>()).MustHaveHappened();
        }

        [Fact]
        public void AllowsDefinitionOfSolveStrategyForQuestionsWithParameterAndResultMapping()
        {
            var definition = A.Fake<ISolutionDefinitionHost>();

            this.testee.Load(definition);

            A.CallTo(() => definition.SolveWithResultMapping<TestQuestionWithParameterAndResultMapping, string, int, char>()).MustHaveHappened();
        }

        private class TestableModule : EvaluationEngineModule
        {
            protected override void Load()
            {
                this.Solve<TestQuestion, string>();
                this.Solve<TestQuestionWithParameter, string, int>();
                this.SolveWithResultMapping<TestQuestionWithResultMapping, string, char>();
                this.SolveWithResultMapping<TestQuestionWithParameterAndResultMapping, string, int, char>();
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class TestQuestion : Question<string>
        {
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class TestQuestionWithParameter : Question<string, int>
        {
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class TestQuestionWithResultMapping : Question<string>
        {
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class TestQuestionWithParameterAndResultMapping : Question<string, int>
        {
        }
    }
}