//-------------------------------------------------------------------------------
// <copyright file="Validation.cs" company="Appccelerate">
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

namespace Appccelerate.EvaluationEngine.Validation
{
    using System.Linq;

    using Appccelerate.EvaluationEngine.Expressions;

    using FluentAssertions;

    using Xbehave;

    public class Validation
    {
        const string ViolationReason = "Name is empty";

        [Scenario]
        public void ValidData(
            EvaluationEngine engine)
        {
            IValidationResult answer = null;

            "establish an evaluation engine with validation answer"._(() =>
            {
                engine = new EvaluationEngine();

                engine.Solve<IsDataValid, IValidationResult, Data>()
                    .AggregateWithValidationAggregator()
                    .ByEvaluating(q => new NameSetRule())
                    .ByEvaluating(q => new DescriptionSetRule());
            });

            "when validating valid data"._(() =>
            {
                answer = engine.Answer(
                new IsDataValid(), 
                new Data
                    {
                        Name = "Tester"
                    });
            });

            "it should return valid validation result"._(() =>
            {
                answer.Valid.Should().BeTrue();
            });

            "it should return validation result without violations"._(() =>
            {
                answer.Violations.Should().BeEmpty();
            });
        }

        [Scenario]
        public void InvalidData(
            EvaluationEngine engine)
        {
            IValidationResult answer = null;

            "establish"._(() =>
            {
                engine = new EvaluationEngine();

                engine.Solve<IsDataValid, IValidationResult, Data>()
                    .AggregateWithValidationAggregator()
                    .ByEvaluating(q => new NameSetRule())
                    .ByEvaluating(q => new DescriptionSetRule());
            });

            "when validating invalid data"._(() =>
            {
                answer = engine.Answer(
                new IsDataValid(), 
                new Data
                    {
                        Name = null, 
                    });
            });

            "it should return invalid validation result"._(() =>
            {
                answer.Valid.Should().BeFalse();
            });

            "it should return validation result with violations"._(() =>
            {
                answer.Violations.Should().HaveCount(1);
            });

            "it should return violations with reason set by failing rule"._(() =>
            {
                answer.Violations.Single().Reason.Should().Be(ViolationReason);
            });
        }

        protected class IsDataValid : IQuestion<IValidationResult, Data>
        {
            public string Describe()
            {
                return "Is data valid?";
            }
        }

        protected class Data
        {
            public string Name { get; set; }
        }

        protected class NameSetRule : EvaluationExpression<IValidationResult, Data>
        {
            public override IValidationResult Evaluate(Data data)
            {
                var result = new ValidationResult();

                if (string.IsNullOrEmpty(data.Name))
                {
                    result.Valid = false;
                    result.AddViolation(new ValidationViolation { Reason = ViolationReason });
                }

                return result;
            }
        }

        protected class DescriptionSetRule : EvaluationExpression<IValidationResult, Data>
        {
            public override IValidationResult Evaluate(Data data)
            {
                return new ValidationResult();
            }
        }
    }
}