//-------------------------------------------------------------------------------
// <copyright file="Strategies.cs" company="Appccelerate">
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

namespace Appccelerate.EvaluationEngine
{
    using System.Collections.Generic;
    using System.Reflection;

    using Appccelerate.EvaluationEngine.Expressions;

    using FakeItEasy;

    using FluentAssertions;

    using Xbehave;

    public class Strategies
    {
        public const int TheAnswer = 42;

        [Scenario]
        public void CustomStrategy(
            IEvaluationEngine engine,
            int answer)
        {
            "establish an evaluatione engine"._(() =>
            {
                engine = new EvaluationEngine();
            });

            "when defining an own strategy"._(() =>
            {
                engine.Solve<HowManyFruitsAreThere, int>()
                    .With(new SpecialStrategy());

                answer = engine.Answer(new HowManyFruitsAreThere());
            });

            "it should use own strategy instead of default strategy to answer the question"._(() =>
            {
                answer.Should().Be(TheAnswer);
            });
        }

        [Scenario]
        public void AggregatorStrategy(
            IEvaluationEngine engine,
            IAggregator<int, int, Missing> aggregator,
            int answer)
        {
            "establish an evaluatione engine"._(() =>
            {
                engine = new EvaluationEngine();

                aggregator = A.Fake<IAggregator<int, int, Missing>>();
                A.CallTo(() => aggregator.Aggregate(A<IEnumerable<IExpression<int, Missing>>>._, Missing.Value, A<Context>._)).Returns(TheAnswer);
            });

            "when defining aggregator strategy"._(() =>
            {
                engine.Solve<HowManyFruitsAreThere, int>()
                    .WithAggregatorStrategy()
                    .AggregateWith(aggregator);

                answer = engine.Answer(new HowManyFruitsAreThere());
            });

            "it should use aggregator strategy to answer the question"._(() =>
            {
                answer.Should().Be(TheAnswer);
            });
        }

        private class SpecialStrategy : IStrategy<int>
        {
            public int Execute(IQuestion<int, Missing> question, Missing parameter, IDefinition definition, Context context)
            {
                return TheAnswer;
            }

            public string Describe()
            {
                return "my own special strategy";
            }
        }
    }
}