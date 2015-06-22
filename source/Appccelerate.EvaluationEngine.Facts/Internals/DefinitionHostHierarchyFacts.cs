//-------------------------------------------------------------------------------
// <copyright file="DefinitionHostHierarchyFacts.cs" company="Appccelerate">
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
    using FluentAssertions.Primitives;

    using Xunit;

    public class DefinitionHostHierarchyFacts
    {
        private readonly DefinitionHost testee;
        private readonly DefinitionHost parent;

        public DefinitionHostHierarchyFacts()
        {
            this.parent = new DefinitionHost();
            this.testee = new DefinitionHost(this.parent);
        }

        [Fact]
        public void ReturnsCloneOfDefinitionOfParent_WhenMatchingDefinitionInParentAndNoMatchingDefinitionInTestee()
        {
            IDefinition definitionOfParent = CreateDefinition();
            this.parent.AddDefinition(definitionOfParent);

            IDefinition definition = this.testee.FindInHierarchyAndCloneDefinition(new TestQuestion());

            definition.Should().BeACloneOf(definitionOfParent);
        }

        [Fact]
        public void ReturnsNull_WhenNoMatchingDefinitionInParentAndNoMatchingDefinitionInTestee()
        {
            var definition = this.testee.FindInHierarchyAndCloneDefinition(new TestQuestion());

            definition.Should().BeNull();
        }

        [Fact]
        public void ReturnsCloneOfDefinitionOfTestee_WhenNoMatchingDefinitionInParentAndMatchingDefinitionInTestee()
        {
            var definitionOfTestee = CreateDefinition();
            this.testee.AddDefinition(definitionOfTestee);

            var definition = this.testee.FindInHierarchyAndCloneDefinition(new TestQuestion());
            
             definition.Should().BeACloneOf(definitionOfTestee);
        }

        [Fact]
        public void ReturnsMergedAndClonedDefinition_WhenMatchingDefinitionInParentAndMatchingDefinitionInTestee()
        {
            var definitionOfParent = CreateDefinition();
            this.parent.AddDefinition(definitionOfParent);

            var definitionOfTestee = CreateDefinition();
            this.testee.AddDefinition(definitionOfTestee);

            var definition = this.testee.FindInHierarchyAndCloneDefinition(new TestQuestion());

            definition.Should().BeACloneOf(definitionOfTestee);
        }

        private static Definition<TestQuestion, string, Missing, string> CreateDefinition()
        {
            var definition = new Definition<TestQuestion, string, Missing, string>
                {
                    Strategy = A.Fake<IStrategy<string, Missing>>(),
                    Aggregator = A.Fake<IAggregator<string, string, Missing>>()
                };

            return definition;
        }
    }

    public static class CloneExtensionMethods
    {
        public static void BeACloneOf(this ObjectAssertions assertion, IDefinition original)
        {
            assertion.Subject.As<Definition<TestQuestion, string, Missing, string>>()
                .Should().ShouldBeEquivalentTo(original, o => o.ExcludingMissingMembers());

            assertion.Subject
                .Should().NotBeSameAs(original, "a clone is expected so that original definition cannot be modified.");
        }
    }

    public class TestQuestion : Question<string>
    {
    }
}