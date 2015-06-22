//-------------------------------------------------------------------------------
// <copyright file="DefinitionHostAnswerFacts.cs" company="Appccelerate">
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
    using System.Reflection;
    using FakeItEasy;
    using FluentAssertions;

    using Xunit;

    public class DefinitionHostAnswerFacts
    {
        private readonly DefinitionHost testee;

        public DefinitionHostAnswerFacts()
        {
            this.testee = new DefinitionHost();
        }

        [Fact]
        public void ReturnsCloneOfDefinition_WhenMatchingDefinitionWasAdded()
        {
            var definition = this.AddDefinition<TestQuestion>();

            var result = this.testee.FindInHierarchyAndCloneDefinition(new TestQuestion());

            result
                .Should().ShouldBeEquivalentTo(definition, o => o.ExcludingMissingMembers());

            result
                .Should().NotBeSameAs(definition, "a clone should be returned so that the original object cannot be changed.");
        }

        [Fact]
        public void ReturnsNull_WhenNoMatchingDefinitionWasAdded()
        {
            var result = this.testee.FindInHierarchyAndCloneDefinition(new TestQuestion());

            result.Should().BeNull();
        }

        private Definition<TQuestion, string, Missing, string> AddDefinition<TQuestion>() where TQuestion : IQuestion<string>
        {
            var definition = new Definition<TQuestion, string, Missing, string>
                {
                    Strategy = A.Fake<IStrategy<string, Missing>>(),
                    Aggregator = A.Fake<IAggregator<string, string, Missing>>()
                };

            this.testee.AddDefinition(definition);
            
            return definition;
        }

        private class TestQuestion : Question<string>
        {
        }
    }
}