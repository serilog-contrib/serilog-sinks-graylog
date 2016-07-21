using FluentAssertions;
using Serilog.Sinks.Graylog.Extensions;
using Xunit;

namespace Serilog.Sinks.Graylog.Tests.Extensions
{
    public class StringExtensionsFixture
    {
        [Fact]
        public void WhenCompressMessage_ThenResultShoouldBeExpected()
        {
            var giwen = "Some string";
            var expected = new byte[]
            {
                31,139,8,0,0,0,0,0,4,0,11,206,207,77,85,40,46,41,202,204,75,7,0,142,183,209,127,11,0,0,0
            };

            byte[] actual = giwen.Compress();
            actual.ShouldAllBeEquivalentTo(expected);
        }
    }
}