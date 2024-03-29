﻿using DG.Common.Threading;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace DG.Common.Tests.Threading
{
    public class TaskLockTests
    {
        [Fact]
        public void UpdateValue_Corrupts()
        {
            var state = new CorruptStateExample();
            var task1 = state.UpdateValueAsync();
            var task2 = state.UpdateValueAsync();

            Task.WaitAll(task1, task2);

            state.Value.Should().NotBe(2);
        }

        [Fact]
        public void UpdateValue_LockedWorks()
        {
            var state = new CorruptStateExample();
            var task1 = state.UpdateValueLockedAsync();
            var task2 = state.UpdateValueLockedAsync();

            Task.WaitAll(task1, task2);

            state.Value.Should().Be(2);
        }

        [Fact]
        public void UpdateValue_GenericLockedWorks()
        {
            var state = new CorruptStateExample();
            var task1 = state.UpdateValueGenericLockedAsync();
            var task2 = state.UpdateValueGenericLockedAsync();

            Task.WaitAll(task1, task2);

            state.Value.Should().Be(2);
        }

        private class CorruptStateExample
        {
            private readonly TaskLock _lock = new TaskLock();
            private int _value;
            public int Value => _value;

            private async Task<int> GetNextValueAsync(int current)
            {
                await Task.Delay(100);
                return current + 1;
            }

            public async Task UpdateValueAsync()
            {
                _value = await GetNextValueAsync(_value);
            }

            public async Task UpdateValueLockedAsync()
            {
                await _lock.LockAsync(async () =>
                {
                    _value = await GetNextValueAsync(_value);
                });
            }

            public async Task UpdateValueGenericLockedAsync()
            {
                _value = await _lock.LockAsync(async () =>
                {
                    return await GetNextValueAsync(_value);
                });
            }
        }
    }
}
