//-------------------------------------------------------------------------------
// <copyright file="Modules.cs" company="Appccelerate">
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

    public class Modules
    {
        private const int NumberOfAnanas = 3;
        private const int NumberOfApples = 2;

        [Scenario]
        public void LoadModule(
            IEvaluationEngine testee)
        {
            "establish an evaluation engine"._(() =>
            {
                testee = new EvaluationEngine();
                testee.Solve<HowManyFruitsAreThere, int>()
                    .AggregateWithExpressionAggregator(0, (aggregate, value) => aggregate + value)
                    .ByEvaluating(q => new FruitCountExpression { Kind = "Apples", NumberOfFruits = NumberOfApples });
            });

            "when loading a module into an evaluation engine"._(() =>
            {
                IEvaluationEngineModule module = new FruitModule();
                testee.Load(module);
            });

            "it should use definitions from module to answer questions too"._(() =>
            {
                int answer = testee.Answer(new HowManyFruitsAreThere());

                answer.Should().Be(NumberOfApples + NumberOfAnanas);
            });
        }

        private class FruitModule : EvaluationEngineModule
        {
            protected override void Load()
            {
                this.Solve<HowManyFruitsAreThere, int>()
                    .ByEvaluating(q => new FruitCountExpression { Kind = "Ananas", NumberOfFruits = NumberOfAnanas });
            }
        }
    }
}