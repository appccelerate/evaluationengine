//-------------------------------------------------------------------------------
// <copyright file="HierarchicalEvaluationEngines.cs" company="Appccelerate">
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

    public class HierarchicalEvaluationEngines
    {
        const string ParentAggregator = "parentAggregator";
        const string ChildAggregator = "childAggregator";
        const string ParentExpression = "parentExpression";
        const string ChildExpression = "childExpression";

        [Scenario]
        public void AskingChild(
            string childAnswer,
            IEvaluationEngine parentEngine,
            IEvaluationEngine childEngine)
        {
            "establish an hierarchical evaluation engine"._(() =>
            {
                parentEngine = new EvaluationEngine();
                childEngine = new EvaluationEngine(parentEngine);

                parentEngine.Solve<Question, string>()
                .AggregateWithExpressionAggregator(ParentAggregator, (aggregate, value) => aggregate + value)
                .ByEvaluating((q, p) => ParentExpression);

                childEngine.Solve<Question, string>()
                    .AggregateWithExpressionAggregator(ChildAggregator, (aggregate, value) => aggregate + value)
                    .ByEvaluating((q, p) => ChildExpression);
            });

            "when calling answer on a child evaluation engine"._(() =>
            {
                childAnswer = childEngine.Answer(new Question());
            });

            "it should override parent aggregator with child aggregator"._(() =>
            {
                childAnswer.Should().Contain(ChildAggregator);
            });

            "it should use expressions from child and parent"._(() =>
            {
                childAnswer
                .Should().Contain(ParentExpression)
                .And.Contain(ChildExpression);
            });

            "it should evaluate expressions from parent first"._(() =>
            {
                childAnswer.EndsWith(ParentExpression + ChildExpression, StringComparison.Ordinal)
                .Should().BeTrue();
            });
        }

        [Scenario]
        public void AskingParent(
            string parentAnswer,
            IEvaluationEngine parentEngine,
            IEvaluationEngine childEngine)
        {
            "establish an hierarchical evaluation engine"._(() =>
            {
                parentEngine = new EvaluationEngine();
                childEngine = new EvaluationEngine(parentEngine);

                parentEngine.Solve<Question, string>()
                .AggregateWithExpressionAggregator(ParentAggregator, (aggregate, value) => aggregate + value)
                .ByEvaluating((q, p) => ParentExpression);

                childEngine.Solve<Question, string>()
                    .AggregateWithExpressionAggregator(ChildAggregator, (aggregate, value) => aggregate + value)
                    .ByEvaluating((q, p) => ChildExpression);
            });

            "when calling answer on a parent evaluation engine"._(() =>
            {
                parentAnswer = parentEngine.Answer(new Question());
            });

            "it should use parent aggregator"._(() =>
            {
                parentAnswer.Should().Contain(ParentAggregator);
            });

            "it should use expressions only from parent"._(() =>
            {
                parentAnswer.Should().NotContain(ChildExpression);
            });
        }

        private class Question : IQuestion<string>
        {
            public string Describe()
            {
                return "test question";
            }
        }
    }
}