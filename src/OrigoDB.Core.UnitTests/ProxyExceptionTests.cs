﻿using System;
using NUnit.Framework;
using OrigoDB.Core;
using OrigoDB.Core.Proxy;
using OrigoDB.Core.Test;

namespace OrigoDB.Core.Test
{
    [TestFixture]
    public class ProxyExceptionTests
    {

        ProxyExceptionTestModel _proxy;
        Engine<ProxyExceptionTestModel> _engine;
        int _callsToExecutíng, _callsToExecuted;

        [SetUp]
        public void Setup()
        {
            var cfg = EngineConfiguration.Create().ForIsolatedTest();
            _engine = Engine.Create<ProxyExceptionTestModel>(cfg);
            _proxy = _engine.GetProxy();
            _callsToExecutíng = 0;
            _callsToExecuted = 0;
            _engine.CommandExecuting += (sender, args) => _callsToExecutíng++;
            _engine.CommandExecuted += (sender, args) => _callsToExecuted++;
            
        }

        [Test]
        public void CommandAbortedException()
        {
            try
            {
                _proxy.ModifyAndThrow(new CommandAbortedException());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<CommandAbortedException>(ex);
                Assert.AreEqual(1, _callsToExecutíng);
                Assert.AreEqual(0, _callsToExecuted);

                //verify that the model wasn't rolled back
                Assert.AreEqual(1, _proxy.GetState());
                return;
            }
            Assert.Fail("Expected exception");
        }

        [Test]
        public void UnexpectedException()
        {
            try
            {
                _proxy.ModifyAndThrow(new ArgumentException());
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<CommandFailedException>(ex);
                Assert.IsNotNull(ex.InnerException, "InnerException was null");
                Assert.IsInstanceOf<ArgumentException>(ex.InnerException, "InnerException was not ArgumentException");
                Assert.AreEqual(1, _callsToExecutíng, "CommandExecuting was not called");
                Assert.AreEqual(0, _callsToExecuted, "CommandExecuted should not have been called");

                Assert.AreEqual(0, _proxy.GetState(), "state was not rolled back");
                return;
            }
            Assert.Fail("Expected exception");
        }

    }


    [Serializable]
    public class ProxyExceptionTestModel : Model
    {
        private int _state;
        public void ModifyAndThrow(Exception ex)
        {
            _state++;
            throw ex;
        }

        public int GetState()
        {
            return _state;
        }
    }
}