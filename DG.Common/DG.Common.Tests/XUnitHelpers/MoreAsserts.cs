using System;
using Xunit;

namespace DG.Common.Tests.XUnitHelpers
{
    public static class MoreAsserts
    {
        public static void NoExceptions(Action testCode)
        {
            var exception = Record.Exception(testCode);
            Assert.Null(exception);
        }
    }
}
