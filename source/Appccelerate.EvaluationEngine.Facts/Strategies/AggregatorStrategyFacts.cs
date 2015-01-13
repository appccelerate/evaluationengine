//-------------------------------------------------------------------------------
// <copyright file="AggregatorStrategyFacts.cs" company="Appccelerate">
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

namespace Appccelerate.EvaluationEngine.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Appccelerate.EvaluationEngine.ExpressionProviders;
    using Appccelerate.EvaluationEngine.Expressions;
    using FakeItEasy;
    using FluentAssertions;

    using Xunit;

    public class AggregatorStrategyFacts
    {
        [Fact]
        public void ExecutesAggregator()
        {
            const string Answer = "42";

            var testee = new AggregatorStrategy<TestQuestion, string, int>();

            var context = new Context();
            var aggregator = A.Fake<IAggregator<string, string, int>>();

            var expressionProvider = A.Fake<IExpressionProvider<TestQuestion, string, int, string>>();
            var question = new TestQuestion();
            var expression = new TestExpression<string>();
            A.CallTo(() => expressionProvider.GetExpressions(question)).Returns(new[] { expression });
            
            var definition = new TestableDefinition<string>
                {
                    Aggregator = aggregator,
                    ExpressionProviders = new[] { expressionProvider }
                };

            const int Parameter = 7;

            A.CallTo(() => aggregator.Aggregate(
                A<IEnumerable<IExpression<string, int>>>.That.Matches(_ => _.Contains(expression) && _.Count() == 1), 
                Parameter, 
                A<Context>._)).Returns(Answer);

            string result = testee.Execute(question, Parameter, definition, context);

            result.Should().Be(Answer);
        }

        [Fact]
        public void ExecutesAggregator_WhenStrategyWithMappingIsUsed()
        {
            const string Answer = "42";

            var testee = new AggregatorStrategy<TestQuestion, string, int, int>();

            var context = new Context();
            var aggregator = A.Fake<IAggregator<int, string, int>>();

            var expressionProvider = A.Fake<IExpressionProvider<TestQuestion, string, int, int>>();
            var question = new TestQuestion();
            var expression = new TestExpression<int>();
            A.CallTo(() => expressionProvider.GetExpressions(question)).Returns(new[] { expression });
            var definition = new TestableDefinition<int>
            {
                Aggregator = aggregator,
                ExpressionProviders = new[] { expressionProvider }
            };
            
            const int Parameter = 7;

            A.CallTo(() => aggregator.Aggregate(
                A<IEnumerable<IExpression<int, int>>>.That.Matches(_ => _.Contains(expression) && _.Count() == 1),
                Parameter,
                A<Context>._)).Returns(Answer);

            string result = testee.Execute(question, Parameter, definition, context);

            result.Should().Be(Answer);
        }

        [Fact]
        public void Describe()
        {
            var testee = new AggregatorStrategy<TestQuestion, string, int>();

            var description = testee.Describe();

            description.Should().Be("aggregator strategy");
        }

        // ReSharper disable once MemberCanBePrivate.Global because of faking
        public class TestQuestion : Question<string, int>
        {
        }

        private class TestExpression<TExpressionResult> : EvaluationExpression<TExpressionResult, int>
        {
            public override TExpressionResult Evaluate(int parameter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestableDefinition<TExpressionResult> : IDefinition<TestQuestion, string, int, TExpressionResult>
        {
            public Type QuestionType
            {
                get { throw new NotImplementedException(); }
            }

            public IStrategy<string, int> Strategy { get; set; }

            public IAggregator<TExpressionResult, string, int> Aggregator { get; set; }

            public IEnumerable<IExpressionProvider<TestQuestion, string, int, TExpressionResult>> ExpressionProviders { get; set; }

            public IStrategy<TAnswer, TParameter> GetStrategy<TAnswer, TParameter>()
            {
                throw new NotImplementedException();
            }

            public IDefinition Clone()
            {
                throw new NotImplementedException();
            }

            public void Merge(IDefinition definition)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IExpressionProvider<TestQuestion, string, int, TExpressionResult>> GetExpressionProviders(IQuestion<string, int> question)
            {
                return this.ExpressionProviders;
            }

            public void AddExpressionProviderSet(IExpressionProviderSet<TestQuestion, string, int, TExpressionResult> expressionProviderSet)
            {
                throw new NotImplementedException();
            }
        }
    }
}