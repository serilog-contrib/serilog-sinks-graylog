using System;
using System.Linq;
using System.Security.Cryptography;
using FluentAssertions;
using AutoFixture;
using Serilog.Sinks.Graylog.Core.Helpers;
using Xunit;

namespace Serilog.Sinks.Graylog.Core.Tests.Helpers
{
    public class MessageIdGeneratorFixture
    {
        private readonly Fixture _fixture;

        public MessageIdGeneratorFixture()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void WhenGenerateFromTimeStamp_ThenReturnsExpectedResult()
        {
            DateTime time = DateTime.UtcNow;
            byte[] given = _fixture.CreateMany<byte>(10).ToArray();
            var target = new TimestampMessageIdGenerator();

            byte[] actual = target.GenerateMessageId(given);

            var actticks = BitConverter.ToInt64(actual, 0);
            var actdate = DateTime.FromBinary(actticks);

            actdate.Should().BeCloseTo(time, TimeSpan.FromMilliseconds(200));
        }

        [Fact]
        public void WhenGenerateTimestampFromMd5_ThenReturnsExpected()
        {
            byte[] given = _fixture.CreateMany<byte>(10).ToArray();

            var target = new Md5MessageIdGenerator();

            MD5 md5 = MD5.Create();
            var expected = md5.ComputeHash(given).Take(8).ToArray();

            var actual = target.GenerateMessageId(given);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void WhenResolveMd5Generator_ThenResult_ShouldBeAsExpectedType()
        {
            var resolver = new MessageIdGeneratorResolver();

            IMessageIdGenerator actual = resolver.Resolve(MessageIdGeneratorType.Md5);

            Assert.IsType<Md5MessageIdGenerator>(actual);
        }

        [Fact]
        public void WhenResolveTimeStampGenerator_ThenResult_ShouldBeAsExpectedType()
        {
            var resolver = new MessageIdGeneratorResolver();

            IMessageIdGenerator actual = resolver.Resolve(MessageIdGeneratorType.Timestamp);

            Assert.IsType<TimestampMessageIdGenerator>(actual);
        }
    }
}