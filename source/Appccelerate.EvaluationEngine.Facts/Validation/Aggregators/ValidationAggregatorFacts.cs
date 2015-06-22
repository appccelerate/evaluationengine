//-------------------------------------------------------------------------------
// <copyright file="ValidationAggregatorFacts.cs" company="Appccelerate">
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

namespace Appccelerate.EvaluationEngine.Validation.Aggregators
{
    using System.Linq;

    using Appccelerate.EvaluationEngine.Expressions;

    using FluentAssertions;

    using Xunit;

    public class ValidationAggregatorFacts
    {
        private readonly ValidationAggregator<int> testee;

        public ValidationAggregatorFacts()
        {
            this.testee = new ValidationAggregator<int>();
        }

        // TODO: test that parameter is passed to expressions.
        [Fact]
        public void ReturnsValid_WhenAllExpressionsAreValid()
        {
            var expressions = new[] 
            { 
                CreateExpression(true),
                CreateExpression(true)
            };

            var answer = this.testee.Aggregate(expressions, 7, null);
            bool valid = answer.Valid;

            valid.Should().BeTrue();
        }

        [Fact]
        public void ReturnsInvalid_WhenAtLeastOneExpressionReturnsInvalid()
        {
            var expressions = new[] 
            { 
                CreateExpression(true),
                CreateExpression(false),
                CreateExpression(true)
            };

            var answer = this.testee.Aggregate(expressions, 7, null);
            bool valid = answer.Valid;

            valid.Should().BeFalse();
        }

        [Fact]
        public void CombinesViolations_WhenSeveralExpressionsHaveViolations()
        {
            const string Reason1 = "a reason";
            const string Reason2 = "another reason";
            const string Reason3 = "yet another reason";

            var expressions = new[] 
            { 
                CreateExpression(true),
                CreateExpression(false, CreateViolation(Reason1)),
                CreateExpression(false, CreateViolation(Reason2), CreateViolation(Reason3))
            };

            var answer = this.testee.Aggregate(expressions, 7, null);
            var violations = answer.Violations.ToList();

            violations.Select(v => v.Reason)
                .Should().Equal(Reason1, Reason2, Reason3);
        }

        [Fact]
        public void IgnoresExpressionsReturningNull()
        {
            var expressions = new[] 
            { 
                CreateExpression(true),
                new TestableExpression { ValidationResult = null },
                CreateExpression(true)
            };

            var answer = this.testee.Aggregate(expressions, 7, null);
            bool valid = answer.Valid;

            valid.Should().BeTrue();
        }

        [Fact]
        public void DescribesItself()
        {
            string description = this.testee.Describe();

            description.Should().Be("validation aggregator");
        }

        private static IExpression<IValidationResult, int> CreateExpression(bool valid, params IValidationViolation[] violations)
        {
            var validationResult = new ValidationResult { Valid = valid };

            if (violations != null)
            {
                foreach (var violation in violations)
                {
                    validationResult.AddViolation(violation);
                }
            }

            return new TestableExpression { ValidationResult = validationResult };
        }

        private static IValidationViolation CreateViolation(string reason)
        {
            return new ValidationViolation { Reason = reason };
        }

        private class TestableExpression : EvaluationExpression<IValidationResult, int>
        {
            public ValidationResult ValidationResult { get; set; }

            public override IValidationResult Evaluate(int parameter)
            {
                return this.ValidationResult;
            }
        }
    }
}