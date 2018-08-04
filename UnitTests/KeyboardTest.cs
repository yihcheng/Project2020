using System.Collections.Generic;
using Abstractions;
using Moq;
using WindowsComputer;
using Xunit;

namespace UnitTests
{
    public class KeyboardTest
    {
        [Theory]
        [InlineData("abc", "abc")]
        [InlineData("123", "123")]
        [InlineData("ctrl+c", "^c")]
        [InlineData("ctrl+cv", "^(cv)")]
        [InlineData("Ctrl+D", "^D")]
        [InlineData("Ctrl+cDe", "^(cDe)")]
        [InlineData("", "")]
        [InlineData("shift+c", "+c")]
        [InlineData("shift+E", "+E")]
        [InlineData("shift+cd", "+(cd)")]
        [InlineData("shift+cDe", "+(cDe)")]
        [InlineData("alt+c", "%c")]
        [InlineData("alt+A", "%A")]
        [InlineData("alt+cd", "%(cd)")]
        [InlineData("alt+cDe", "%(cDe)")]
        public void ConvertKeyTest(string testString, string expectedString)
        {
            Mock<IMouse> mouseMock = CreateMouseMock();
            WindowsKeyboard keyboard = new WindowsKeyboard(mouseMock.Object);
            string convertedKey = keyboard.ConvertKey(testString);

            Assert.Equal(expectedString, convertedKey);
        }

        private static Mock<IMouse> CreateMouseMock()
        {
            Mock<IMouse> mouse = new Mock<IMouse>();
            mouse.Setup(m => m.Click(It.IsAny<int>(), It.IsAny<int>()));

            return mouse;
        }

        [Theory]
        [MemberData(nameof(DivideKeyData))]
        public void DivideKeyTest(string keyString, List<string> expectedKeys)
        {
            Mock<IMouse> mouseMock = CreateMouseMock();
            WindowsKeyboard keyboard = new WindowsKeyboard(mouseMock.Object);
            List<string> actualKeys = keyboard.DivideKeyString(keyString);

            Assert.Equal(expectedKeys.Count, actualKeys.Count);

            for (int i = 0; i < expectedKeys.Count; i++)
            {
                Assert.Equal(expectedKeys[i], actualKeys[i]);
            }
        }

        public static IEnumerable<object[]> DivideKeyData =>
            new List<object[]>
            {
                new object[] { "", new List<string>()},
                new object[] { "abc", new List<string>(){ "abc"} },
                new object[] { "abc[ctrl+c]", new List<string>(){ "abc", "ctrl+c"} },
                new object[] { "abc[ctrl+c]123[alt+d]", new List<string>(){ "abc", "ctrl+c", "123", "alt+d"} },
                new object[] { "abc[ctrl+Cd]123[alt+dE]", new List<string>(){ "abc", "ctrl+Cd", "123", "alt+dE"} }
            };
    }
}
