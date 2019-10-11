// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Memory
{
    public class RollingMemory : IDisposable
    {
#pragma warning disable IDE0069 // Disposable fields should be disposed
        private readonly MemoryPool<byte> _memoryPool;
        private RollingMemorySegment _firstSegment;
        private int _firstIndex;
        private RollingMemorySegment _lastSegment;
        private int _lastIndex;
#pragma warning restore IDE0069 // Disposable fields should be disposed

        public RollingMemory(MemoryPool<byte> memoryPool)
        {
            _memoryPool = new NeverRentZeroMemoryPoolWrapper<byte>(memoryPool);
        }

        public bool IsEmpty => _firstSegment == _lastSegment && _firstIndex == _lastIndex;

        public void Dispose()
        {
            // TODO: put object disposed guards in front of operations
            var segment = _firstSegment;
            _firstSegment = null;
            _lastSegment = null;
            while (segment != null)
            {
                var nextSegment = segment.Next;
                segment.Dispose();
                segment = nextSegment;
            }
        }

        public ReadOnlySequence<byte> GetOccupiedMemory()
        {
            if (_firstSegment == null && _lastSegment == null)
            {
                return ReadOnlySequence<byte>.Empty;
            }

            return new ReadOnlySequence<byte>(_firstSegment, _firstIndex, _lastSegment, _lastIndex);
        }

        public void ConsumeOccupiedMemory(SequencePosition consumed)
        {
            var consumedSegment = (RollingMemorySegment)consumed.GetObject();
            var consumedIndex = consumed.GetInteger();

            var firstPosition = _firstSegment.RunningIndex + _firstIndex;
            var consumedPosition = consumedSegment.RunningIndex + consumedIndex;

            var consumedCount = consumedPosition - firstPosition;
            ConsumeOccupiedMemory(consumedCount);
        }

        public void ConsumeOccupiedMemory(long consumed)
        {
            var remaining = consumed;
            while (remaining != 0)
            {
                var occupied = _firstSegment.Memory.Length - _firstIndex;
                if (remaining < occupied)
                {
                    _firstIndex += (int)remaining;
                    break;
                }

                DisposeFirstSegment();
                remaining -= occupied;
            }

            if (_firstSegment != null && _firstIndex == _firstSegment.Memory.Length)
            {
                DisposeFirstSegment();
            }
        }

        public void DisposeFirstSegment()
        {
            if (_firstSegment == null && _lastSegment == null)
            {
                return;
            }

            var disposedSegment = _firstSegment;

            if (_firstSegment == _lastSegment)
            {
                _firstSegment = null;
                _firstIndex = 0;
                _lastSegment = null;
                _lastIndex = 0;
            }
            else
            {
                _firstSegment = disposedSegment.Next;
                _firstIndex = 0;
            }

            disposedSegment.Dispose();
        }

        public Memory<byte> GetTrailingMemory(int sizeHint = 0)
        {
            // special case, no buffers.
            // allocate buffer on first read.
            if (_firstSegment == null && _lastSegment == null)
            {
                var rental = _memoryPool.Rent(sizeHint);
                var segment = new RollingMemorySegment(rental, 0);
                _firstSegment = segment;
                _lastSegment = segment;
                return rental.Memory;
            }

            // special case, all occupied memory has been consumed.
            // drop both index to 0 so the entire page becomes trailing memory again.
            if (IsEmpty)
            {
                _firstIndex = 0;
                _lastIndex = 0;
            }

            // special case, last page is completely full.
            // allocate and append a new unoccupied last page.
            if (_lastIndex == _lastSegment.Memory.Length)
            {
                var rental = _memoryPool.Rent(sizeHint);
                _lastSegment.Next = new RollingMemorySegment(
                    rental,
                    _lastSegment.RunningIndex + _lastIndex);
                _lastSegment = _lastSegment.Next;
                _lastIndex = 0;
                return rental.Memory;
            }

            return MemoryMarshal.AsMemory(_lastSegment.Memory.Slice(_lastIndex));
        }

        public bool HasUnexaminedData(SequencePosition examined)
        {
            if (IsEmpty)
            {
                return false;
            }

            var examinedObject = examined.GetObject();
            var examinedInteger = examined.GetInteger();
            if (ReferenceEquals(examinedObject, _lastSegment) &&
                examinedInteger == _lastIndex)
            {
                return false;
            }

            return true;
        }

        public void ConsumeTrailingMemory(int consumed)
        {
            _lastIndex += consumed;
        }

        public void TrailingMemoryFilled(int bytesReceived)
        {
            _lastIndex += bytesReceived;
        }

        private class NeverRentZeroMemoryPoolWrapper<T> : MemoryPool<T>
        {
            private readonly MemoryPool<T> _memoryPool;

            public NeverRentZeroMemoryPoolWrapper(MemoryPool<T> memoryPool)
            {
                _memoryPool = memoryPool;
            }

            public override int MaxBufferSize => _memoryPool.MaxBufferSize;

            public override IMemoryOwner<T> Rent(int minBufferSize = -1) => _memoryPool.Rent(minBufferSize > 0 ? minBufferSize : -1);

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _memoryPool.Dispose();
                }
            }
        }
    }
}
