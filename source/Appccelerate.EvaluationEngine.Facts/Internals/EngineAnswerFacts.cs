//-------------------------------------------------------------------------------
// <copyright file="EngineAnswerFacts.cs" company="Appccelerate">
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

namespace Appccelerate.EvaluationEngine.Internals
{
    using System;

    using Appccelerate.EvaluationEngine.Syntax;
    using FakeItEasy;
    using FluentAssertions;

    using Xunit;

    public class EngineAnswerFacts
    {
        private readonly Engine testee;

        private readonly IDefinitionHost definitionHost;

        public EngineAnswerFacts()
        {
            this.definitionHost = A.Fake<IDefinitionHost>();
            var syntaxFactoryMock = A.Fake<IDefinitionSyntaxFactory>();
            var definitionFactoryMock = A.Fake<IDefinitionFactory>();
            
            this.testee = new Engine(this.definitionHost, syntaxFactoryMock, definitionFactoryMock);
        }

        [Fact]
        public void AnswersQuestionsByExecutingTheStrategyReturnedByTheDefinitionReturnedByTheDefinitionHostForTheQuestion()
        {
            const string Answer = "42";
            const string Parameter = "test";

            var question = new TestQuestion();
            var definition = A.Fake<IDefinition>();
            var strategy = A.Fake<IStrategy<string, string>>();

            A.CallTo(() => this.definitionHost.FindInHierarchyAndCloneDefinition(question)).Returns(definition);
            A.CallTo(() => definition.GetStrategy<string, string>()).Returns(strategy);
            A.CallTo(() => strategy.Execute(question, Parameter, definition, A<Context>._)).Returns(Answer);

            var answer = this.testee.Answer(question, Parameter);

            answer.Should().Be(Answer);
        }

        [Fact]
        public void ThrowsException_WhenNoDefinitionExists()
        {
            var question = new TestQuestion();

            A.CallTo(() => this.definitionHost.FindInHierarchyAndCloneDefinition(question)).Returns((IDefinition)null);
            
            Action action = () => this.testee.Answer(question, string.Empty);

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void ThrowsException_WhenNoStrategyExists()
        {
            var question = new TestQuestion();
            var definition = A.Fake<IDefinition>();
            
            A.CallTo(() => this.definitionHost.FindInHierarchyAndCloneDefinition(question)).Returns(definition);
            A.CallTo(() => definition.GetStrategy<string, string>()).Returns((IStrategy<string, string>)null);

            Action action = () => this.testee.Answer(question, string.Empty);

            action.ShouldThrow<InvalidOperationException>();
        }

        private class TestQuestion : Question<string, string>
        {
        }
    }
}