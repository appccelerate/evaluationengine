//-------------------------------------------------------------------------------
// <copyright file="ExpressionDefinition.cs" company="Appccelerate">
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
    using FluentAssertions;

    using Xbehave;

    public class ExpressionDefinition
    {
        [Scenario]
        public void SeveralCallsWithSingleExpressionToSolve(
            IEvaluationEngine engine,
            int answer)
        {
            "establish an evaluation engine with expression aggregator"._(() =>
            {
                engine = new EvaluationEngine();
                engine.Solve<HowManyFruitsAreThere, int>()
                    .AggregateWithExpressionAggregator(0, (aggregate, value) => aggregate + value);
            });

            "when defining several expressions with individual calls to solve"._(() =>
            {
                engine.Solve<HowManyFruitsAreThere, int>()
                    .ByEvaluating(q => new FruitCountExpression { NumberOfFruits = 2 });

                engine.Solve<HowManyFruitsAreThere, int>()
                    .ByEvaluating(q => new FruitCountExpression { NumberOfFruits = 3 });

                answer = engine.Answer(new HowManyFruitsAreThere());
            });

            "it should evaluate all expressions for the question"._(() =>
            {
                answer.Should().Be(5);
            });
        }
    
        [Scenario]
        public void SeveralExpressionsInSingleCallToSolve(
            IEvaluationEngine engine,
            int answer)
        {
            "establish an evaluation engine with expression aggregator"._(() =>
            {
                engine = new EvaluationEngine();
                engine.Solve<HowManyFruitsAreThere, int>()
                    .AggregateWithExpressionAggregator(0, (aggregate, value) => aggregate + value);
            });
            
            "when defining several expressions with a single call to solve"._(() =>
            {
                engine.Solve<HowManyFruitsAreThere, int>()
                    .ByEvaluating(q => new FruitCountExpression { NumberOfFruits = 2 })
                    .ByEvaluating(q => new FruitCountExpression { NumberOfFruits = 3 });

                answer = engine.Answer(new HowManyFruitsAreThere());
            });

            "it should evaluate all expressions for the question"._(() =>
            {
                answer.Should().Be(5);
            });
        }

        [Scenario]
        public void SeveralExpressionsInSingleCallToByEvaluating(
            IEvaluationEngine engine,
            int answer)
        {
            "establish an evaluation engine with expression aggregator"._(() =>
            {
                engine = new EvaluationEngine();
                engine.Solve<HowManyFruitsAreThere, int>()
                    .AggregateWithExpressionAggregator(0, (aggregate, value) => aggregate + value);
            });

            "when defining several expressions with a single call to by evaluating"._(() =>
            {
                engine.Solve<HowManyFruitsAreThere, int>()
                    .ByEvaluating(q => new[] { new FruitCountExpression { NumberOfFruits = 2 }, new FruitCountExpression { NumberOfFruits = 3 } });

                answer = engine.Answer(new HowManyFruitsAreThere());
            });

            "it should evaluate all expressions for the question"._(() =>
            {
                answer.Should().Be(5);
            });
        }

        [Scenario]
        public void InlineExpressions(
            IEvaluationEngine engine,
            int answer)
        {
            "establish an evaluation engine with expression aggregator"._(() =>
            {
                engine = new EvaluationEngine();
                engine.Solve<HowManyFruitsAreThere, int>()
                    .AggregateWithExpressionAggregator(0, (aggregate, value) => aggregate + value);
            });

            "when defining inline expressions in the call to by evaluating"._(() =>
            {
                engine.Solve<HowManyFruitsAreThere, int>()
                    .ByEvaluating((q, p) => 2)
                    .ByEvaluating((q, p) => 3);

                answer = engine.Answer(new HowManyFruitsAreThere());
            });

            "it should evaluate all expressions for the question"._(() =>
            {
                answer.Should().Be(5);
            });
        }
    }
}