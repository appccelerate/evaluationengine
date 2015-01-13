//-------------------------------------------------------------------------------
// <copyright file="EngineSolveFacts.cs" company="Appccelerate">
//   Copyright (c) 2008-2015
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
    using Appccelerate.EvaluationEngine.Syntax;
    using FakeItEasy;
    using FluentAssertions;

    using Xunit;

    public class EngineSolveFacts
    {
        private readonly Engine testee;

        private readonly IDefinitionHost definitionHost;
        private readonly IDefinitionSyntaxFactory syntaxFactory;
        private readonly IDefinitionFactory definitionFactory;

        public EngineSolveFacts()
        {
            this.definitionHost = A.Fake<IDefinitionHost>();
            this.syntaxFactory = A.Fake<IDefinitionSyntaxFactory>(builder => builder.Strict());
            this.definitionFactory = A.Fake<IDefinitionFactory>();

            this.testee = new Engine(this.definitionHost, this.syntaxFactory, this.definitionFactory);
        }

        [Fact]
        public void PassesExistingDefinitionToTheBuilder_WhenThereAlreadyExistsADefinitionForTheQuestion()
        {
            var builder = A.Fake<IDefinitionSyntax<TestQuestion, string, int, string>>();
            var definition = new Definition<TestQuestion, string, int, string>();

            A.CallTo(() => this.definitionHost.FindDefinition<string>(typeof(TestQuestion))).Returns(definition);
            A.CallTo(() => this.syntaxFactory.CreateDefinitionSyntax(definition)).Returns(builder);
            
            var result = this.testee.Solve<TestQuestion, string, int>();

            result.Should().BeSameAs(builder);
        }

        [Fact]
        public void ANewDefinitionIsPassedToTheBuilderAndStoredInTheDefinitionHost_WhenThereDoesNotExistADefinitionForTheQuestion()
        {
            var builder = A.Fake<IDefinitionSyntax<TestQuestion, string, int, string>>();
            var definition = new Definition<TestQuestion, string, int, string>();

            A.CallTo(() => this.definitionHost.FindDefinition<string>(typeof(TestQuestion))).Returns((IDefinition)null);
            A.CallTo(() => this.definitionFactory.CreateDefinition<TestQuestion, string, int, string>()).Returns(definition);
            A.CallTo(() => this.syntaxFactory.CreateDefinitionSyntax(definition)).Returns(builder);

            var result = this.testee.Solve<TestQuestion, string, int>();

            A.CallTo(() => this.definitionHost.AddDefinition(definition)).MustHaveHappened();
            result.Should().BeSameAs(builder);
        }

        // ReSharper disable once MemberCanBePrivate.Global because of faking
        public class TestQuestion : Question<string, int>
        {
        }
    }
}