using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudtoid.UrlPattern.UnitTests
{
    [TestClass]
    public sealed class PatternVariableTests
    {
        [TestMethod]
        public void IsValidVariableChar_AllValidChars_Success()
        {
            var name = "abcdefghijklmnopqrstvuwxyzABCDEFGHIJKLMNOPQRSTVUWXYZ0123456789_";
            name.All(c => PatternVariables.IsValidVariableChar(c, false)).Should().BeTrue();
        }

        [TestMethod]
        public void TryGetIndex_AllValidChars_Success()
        {
            var name = "abcdefghijklmnopqrstvuwxyzABCDEFGHIJKLMNOPQRSTVUWXYZ0123456789_";
            name.All(c => PatternVariables.ValidVariableCharacters[PatternVariables.TryGetIndex(c, out var i) ? i : -1] == char.ToUpperInvariant(c))
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void TryGetIndex_InvalidChar_Fail()
        {
            PatternVariables.TryGetIndex('}', out var index).Should().BeFalse();
            index.Should().Be(-1);

            PatternVariables.TryGetIndex((char)180, out index).Should().BeFalse();
            index.Should().Be(-1);
        }

        [TestMethod]
        public void IsValidVariableChar_WhenFirstCharIsNumber_Fails()
        {
            for (int i = 48; i < 59; i++)
                PatternVariables.IsValidVariableChar(i, true).Should().BeFalse();
        }

        [TestMethod]
        public void IsValidVariableChar_WhenAnInvalidChar_Success()
        {
            Enumerable.Range(0, 128).Count(c => PatternVariables.IsValidVariableChar(c, false)).Should().Be(63);
        }
    }
}
