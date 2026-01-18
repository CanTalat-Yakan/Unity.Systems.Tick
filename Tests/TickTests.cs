using System;
using NUnit.Framework;
using UnityEssentials;

namespace UnityEssentials.Tests
{
    [TestFixture]
    public class TickTests
    {
        [SetUp]
        public void SetUp()
        {
            // Ensure a clean state before each test
            Tick.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            Tick.Clear();
        }

        [Test]
        public void Register_ThrowsOnNullAction()
        {
            Assert.Throws<ArgumentNullException>(() => Tick.Register(1, null));
        }

        [Test]
        public void Register_ThrowsOnNonPositiveTicksPerSecond()
        {
            Assert.Throws<ArgumentException>(() => Tick.Register(0, () => { }));
            Assert.Throws<ArgumentException>(() => Tick.Register(-5, () => { }));
        }

        [Test]
        public void Register_AddsActionAndDoesNotDuplicate()
        {
            int callCount = 0;
            Action action = () => callCount++;

            Tick.Register(10, action);
            Tick.Register(10, action); // Should not add duplicate

            // Simulate enough time to trigger at least one tick
            Tick.Update(0.1f);

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Unregister_RemovesAction()
        {
            int callCount = 0;
            Action action = () => callCount++;

            Tick.Register(5, action);
            Tick.Unregister(5, action);

            Tick.Update(1.0f);

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Unregister_DoesNothingIfActionNotFound()
        {
            // Should not throw
            Tick.Unregister(5, () => { });
        }

        [Test]
        public void Update_ExecutesRegisteredActionsAtCorrectRate()
        {
            int callCount = 0;
            Tick.Register(2, () => callCount++);

            // 0.5s per tick at 2 ticks/sec, so 1.0s = 2 ticks
            Tick.Update(1.0f);

            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void Update_DoesNotExecuteIfNoActions()
        {
            // Should not throw or do anything
            Tick.Update(1.0f);
        }

        [Test]
        public void Update_RemovesEmptyGroups()
        {
            int callCount = 0;
            Action action = () => callCount++;

            Tick.Register(3, action);
            Tick.Unregister(3, action);

            // Should remove the group internally without error
            Tick.Update(1.0f);

            // No exception, group is cleaned up
            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Clear_ResetsAllState()
        {
            int callCount = 0;
            Action action = () => callCount++;

            Tick.Register(1, action);
            Tick.Clear();

            Tick.Update(1.0f);

            Assert.AreEqual(0, callCount);
        }
    }
}