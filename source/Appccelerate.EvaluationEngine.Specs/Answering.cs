//-------------------------------------------------------------------------------
// <copyright file="Answering.cs" company="Appccelerate">
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
    using System;

    using FluentAssertions;

    using Xbehave;

    public class Answering
    {
        [Scenario]
        public void ParameterlessQuestion(
            IEvaluationEngine evaluationEngine,
            int answer)
        {
            const int NumberOfApples = 3;
            const int NumberOfBananas = 2;

            "establish an evaluation engine"._(() =>
            {
                evaluationEngine = new EvaluationEngine();
            });

            "when calling answer on evaluation engine"._(() =>
            {
                evaluationEngine.Solve<HowManyFruitsAreThere, int>()
                    .AggregateWithExpressionAggregator(0, (aggregate, value) => aggregate + value)
                    .ByEvaluating(q => new FruitCountExpression { Kind = "Apples", NumberOfFruits = NumberOfApples })
                    .ByEvaluating(q => new FruitCountExpression { Kind = "Bananas", NumberOfFruits = NumberOfBananas });

                answer = evaluationEngine.Answer(new HowManyFruitsAreThere());
            });

            "it should aggregate the result of all expressions into a single result"._(() =>
            {
                answer.Should().Be(NumberOfApples + NumberOfBananas);
            });
        }

        [Scenario]
        public void ParametrizedQuestion()
        {
            const string Parameter = "hello world of questions and answers.";

            IEvaluationEngine evaluationEngine = null;

            WordCountExpression expression = null;

            "establish"._(() =>
            {
                evaluationEngine = new EvaluationEngine();
                expression = new WordCountExpression();

                evaluationEngine.Solve<HowManyWordsDoesThisTextHave, int, string>()
                    .AggregateWithSingleExpressionAggregator()
                    .ByEvaluating(q => expression);
            });

            "when calling answer on evaluation engine with a parameter"._(() =>
            {
                evaluationEngine.Answer(new HowManyWordsDoesThisTextHave(), Parameter);
            });

            "it should pass parameter to the evaluation expressions"._(() =>
            {
                expression.ReceivedParameter.Should().Be(Parameter);
            });
        }

        [Scenario]
        public void Constraints(
            IEvaluationEngine engine,
            string answer)
        {
            const string NoConstraint = "N";
            const string WithTrueConstraint = "T";
            const string WithFalseConstraint = "F";

            "establish a solving strategy with constraints"._(() =>
            {
                engine = new EvaluationEngine();

                engine.Solve<WhatIsTheText, string>()
                    .AggregateWithExpressionAggregator(string.Empty, (aggregate, value) => aggregate + value)
                    .ByEvaluating((q, p) => NoConstraint)
                    .When(q => false)
                        .ByEvaluating((q, p) => WithFalseConstraint)
                    .When(q => true)
                        .ByEvaluating((q, p) => WithTrueConstraint);
            });

            "when calling answer"._(() =>
            {
                answer = engine.Answer(new WhatIsTheText());
            });

            "it should evaluate expressions without constraints"._(() =>
            {
                answer.Should().Contain(NoConstraint);
            });

            "it should evaluate expressions with fulfilled constraints"._(() =>
            {
                answer.Should().Contain(WithTrueConstraint);
            });

            "it should ignore expressions with constraints that are not fulfilled"._(() =>
            {
                answer.Should().NotContain(WithFalseConstraint);
            });
        }

        [Scenario]
        public void MissingAggregator()
        {
            IEvaluationEngine engine = null;

            Exception exception = null;

            "establish"._(() =>
            {
                engine = new EvaluationEngine();

                engine.Solve<HowManyFruitsAreThere, int>();
            });

            "when calling answer on evaluation engine and no aggregator is specified"._(() =>
            {
                exception = Catch.Exception(() => engine.Answer(new HowManyFruitsAreThere()));
            });

            "it should throw invalid operation exception"._(() =>
            {
                exception.Should().BeOfType<InvalidOperationException>();
            });
        }
    }
}