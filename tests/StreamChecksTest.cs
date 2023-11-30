using DG.Common.Exceptions;
using DG.Common.Tests.TestHelpers;
using System;
using System.IO;
using Xunit;

namespace DG.Common.Tests
{
    public class StreamChecksTest
    {
        [Fact]
        public void CannotSeek_ShouldThrow()
        {
            var stream = new UnseekableStream();

            Action action = () => ThrowIf.Stream(stream, nameof(stream)).CannotSeek();

            Assert.Throws<ArgumentException>(() => action());
        }

        [Fact]
        public void CannotSeek_ShouldNotThrow()
        {
            var stream = new UnreadableStream();

            Action action = () => ThrowIf.Stream(stream, nameof(stream)).CannotSeek();

            MoreAsserts.NoExceptions(() => action());
        }

        [Fact]
        public void CannotRead_ShouldThrow()
        {
            var stream = new UnreadableStream();

            Action action = () => ThrowIf.Stream(stream, nameof(stream)).CannotRead();

            Assert.Throws<ArgumentException>(() => action());
        }

        [Fact]
        public void CannotRead_ShouldNotThrow()
        {
            var stream = new UnwritableZeroLengthStream();

            Action action = () => ThrowIf.Stream(stream, nameof(stream)).CannotRead();

            MoreAsserts.NoExceptions(() => action());
        }

        [Fact]
        public void CannotWrite_ShouldThrow()
        {
            var stream = new UnwritableZeroLengthStream();

            Action action = () => ThrowIf.Stream(stream, nameof(stream)).CannotWrite();

            Assert.Throws<ArgumentException>(() => action());
        }

        [Fact]
        public void CannotWrite_ShouldNotThrow()
        {
            var stream = new UnreadableStream();

            Action action = () => ThrowIf.Stream(stream, nameof(stream)).CannotWrite();

            MoreAsserts.NoExceptions(() => action());
        }

        [Fact]
        public void IsEmpty_ShouldThrow()
        {
            var stream = new UnwritableZeroLengthStream();

            Action action = () => ThrowIf.Stream(stream, nameof(stream)).IsEmpty();

            Assert.Throws<ArgumentException>(() => action());
        }

        [Fact]
        public void IsEmpty_ShouldNotThrow()
        {
            var stream = new UnreadableStream();

            Action action = () => ThrowIf.Stream(stream, nameof(stream)).IsEmpty();

            MoreAsserts.NoExceptions(() => action());
        }


        private class UnreadableStream : StreamStub
        {
            public override bool CanRead => false;

            public override bool CanSeek => true;

            public override bool CanWrite => true;

            public override long Length => 100;
        }
        private class UnwritableZeroLengthStream : StreamStub
        {
            public override bool CanRead => true;

            public override bool CanSeek => true;

            public override bool CanWrite => false;

            public override long Length => 0;
        }
        private class UnseekableStream : StreamStub
        {
            public override bool CanRead => true;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => 100;
        }

        private abstract class StreamStub : Stream
        {
            public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

            public override void Flush()
            {
                throw new System.NotImplementedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new System.NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new System.NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new System.NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
