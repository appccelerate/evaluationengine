//-------------------------------------------------------------------------------
// <copyright file="DefinitionBuilderFacts.cs" company="Appccelerate">
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

namespace Appccelerate.EvaluationEngine.Syntax
{
    using System.Collections.Generic;
    using System.Linq;

    using Appccelerate.EvaluationEngine.ExpressionProviders;
    using Appccelerate.EvaluationEngine.Expressions;
    using Appccelerate.EvaluationEngine.Internals;
    using FakeItEasy;
    using FluentAssertions;

    using Xunit;

    public class DefinitionBuilderFacts
    {
        private readonly DefinitionBuilder<TestQuestion, string, int, string> testee;

        public DefinitionBuilderFacts()
        {
            var factory = new DefaultFactory();
            var definition = new Definition<TestQuestion, string, int, string>();

            this.testee = new DefinitionBuilder<TestQuestion, string, int, string>(definition, factory);
        }

        [Fact]
        public void With()
        {
            var strategy = A.Fake<IStrategy<string, int>>();

            var syntax = this.testee.With(strategy);

            syntax.Should().BeSameAs(this.testee);
            this.testee.Definition.Strategy.Should().BeSameAs(strategy);
        }

        [Fact]
        public void AggregateWith()
        {
            var aggregator = A.Fake<IAggregator<string, string, int>>();

            var syntax = this.testee.AggregateWith(aggregator);

            syntax.Should().BeSameAs(this.testee);
            this.testee.Definition.Aggregator.Should().BeSameAs(aggregator);
        }

        [Fact]
        public void ByEvaluatingWithSingleExpressionProvider()
        {
            this.testee.ByEvaluating(q => new TestExpression(q));

            var testQuestion = new TestQuestion();
            var expressions = this.GetExpressionsFromTestee(testQuestion).ToList();

            expressions.Should().HaveCount(1);
            expressions.ElementAt(0).Question.Should().BeSameAs(testQuestion);
        }

        [Fact]
        public void ByEvaluatingWithMultipleExpressionsProvider()
        {
            this.testee.ByEvaluating(q => new[] { new TestExpression(q), new TestExpression(q) });

            var testQuestion = new TestQuestion();
            var expressions = this.GetExpressionsFromTestee(testQuestion).ToList();

            expressions.Should().HaveCount(2);
            expressions.ElementAt(0).Question.Should().BeSameAs(testQuestion);
            expressions.ElementAt(1).Question.Should().BeSameAs(testQuestion);
        }

        [Fact]
        public void ByEvaluatingWithInlineExpression()
        {
            this.testee.ByEvaluating((q, p) => "hello");

            var testQuestion = new TestQuestion();
            var expressionProvider = this.testee.Definition.GetExpressionProviders(testQuestion).ToList();

            expressionProvider.Should().HaveCount(1);
            expressionProvider.Single().Should().BeOfType<InlineExpressionProvider<TestQuestion, string, int, string>>();
        }

        [Fact]
        public void When()
        {
            var testQuestion = new TestQuestion();

            this.testee.When(question => question == testQuestion).ByEvaluating(question => new TestExpression(question));
            this.testee.When(question => question != testQuestion).ByEvaluating(question => new TestExpression(null));

            var expressions = this.GetExpressionsFromTestee(testQuestion).ToList();

            expressions.Should().HaveCount(1);
            expressions.Single().Question.Should().BeSameAs(testQuestion);
        }

        private IEnumerable<TestExpression> GetExpressionsFromTestee(TestQuestion question)
        {
            return (from provider in this.testee.Definition.GetExpressionProviders(question)
                    from expression in provider.GetExpressions(question)
                    select expression).Cast<TestExpression>();
        }

        private class TestQuestion : Question<string, int>
        {
        }

        private class TestExpression : EvaluationExpression<string, int>
        {
            public TestExpression(TestQuestion question)
            {
                this.Question = question;
            }

            public TestQuestion Question { get; private set; }

            public override string Evaluate(int parameter)
            {
                return null;
            }
        }
    }
}