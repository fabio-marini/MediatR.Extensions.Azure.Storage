using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace MediatR.Extensions.Tests
{
    public class FactDisplayNameOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var x = testCases.OrderBy(t => t.DisplayName);

            return x;
        }
    }
}
