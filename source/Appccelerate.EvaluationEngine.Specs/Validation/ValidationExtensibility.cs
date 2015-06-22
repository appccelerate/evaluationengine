//-------------------------------------------------------------------------------
// <copyright file="ValidationExtensibility.cs" company="Appccelerate">
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

    public class ValidationExtensibility
    {
        const string NameIsEmptyReason = "Name is empty";
        const string Hint = "A hint";

        public interface IMyValidationResult : IValidationResult<IMyValidationViolation>
        {
        }

        public interface IMyValidationViolation : IValidationViolation
        {
            string ViolationHint { get; set; }
        }

        [Scenario]
        public void ValidData(
            EvaluationEngine engine,
            IMyValidationResult answer)
        {
            "establish an evaluation engine with custom validation"._(() =>
            {
                engine = new EvaluationEngine();

                engine.Solve<IsDataValid, IMyValidationResult, Data>()
                    .AggregateWithValidationAggregator(new MyValidationResultFactory())
                    .ByEvaluating(q => new NameSetRule());
            });

            "when validating valid data with extended validation result"._(() =>
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
            EvaluationEngine engine,
            IMyValidationResult answer)
        {
            "establish an evaluation engine with custom validation"._(() =>
            {
                engine = new EvaluationEngine();

                engine.Solve<IsDataValid, IMyValidationResult, Data>()
                    .AggregateWithValidationAggregator(new MyValidationResultFactory())
                    .ByEvaluating(q => new NameSetRule());
            });

            "when validating invalid data with extended validation result"._(() =>
            {
                answer = engine.Answer(
                new IsDataValid(),
                new Data
                    {
                        Name = null
                    });
            });

            "it should return invalid validation result"._(() =>
            {
                answer.Valid.Should().BeFalse();
            });

            "it should return validation result with violations"._(() =>
            {
                answer.Violations.Should().HaveCount(1);

                answer.Violations.Single().Reason.Should().Be(NameIsEmptyReason);
                answer.Violations.Single().ViolationHint.Should().Be(Hint);
            });

            "it should return violations with reason set by failing rule"._(() =>
            {
                answer.Violations.Single().Reason.Should().Be(NameIsEmptyReason);
            });

            "it should return violations with extended data"._(() =>
            {
                answer.Violations.Single().ViolationHint.Should().Be(Hint);
            });
        }

        private class Data
        {
            public string Name { get; set; }
        }

        private class IsDataValid : IQuestion<IMyValidationResult, Data>
        {
            public string Describe()
            {
                return "Is data valid?";
            }
        }

        private class NameSetRule : MyValidationExpression<Data>
        {
            public override IMyValidationResult Evaluate(Data data)
            {
                var result = this.Factory.CreateValidationResult();

                if (string.IsNullOrEmpty(data.Name))
                {
                    result.Valid = false;

                    var validationViolation = this.Factory.CreateValidationViolation();
                    validationViolation.Reason = ValidationExtensibility.NameIsEmptyReason;
                    validationViolation.ViolationHint = ValidationExtensibility.Hint;
                    result.AddViolation(validationViolation);
                }

                return result;
            }
        }

        private abstract class MyValidationExpression<TParameter> : EvaluationExpression<IMyValidationResult, TParameter>
        {
            protected MyValidationExpression()
            {
                this.Factory = new MyValidationResultFactory();
            }

            protected IValidationResultFactory<IMyValidationResult, IMyValidationViolation> Factory { get; private set; }
        }

        private class MyValidationResult : ValidationResult<IMyValidationViolation>, IMyValidationResult
        {
        }

        private class MyValidationResultFactory : IValidationResultFactory<IMyValidationResult, IMyValidationViolation>
        {
            public IMyValidationResult CreateValidationResult()
            {
                return new MyValidationResult();
            }

            public IMyValidationViolation CreateValidationViolation()
            {
                return new MyValidationViolation();
            }
        }

        private class MyValidationViolation : ValidationViolation, IMyValidationViolation
        {
            public string ViolationHint
            {
                get;
                set;
            }
        }
    }
}