using System;
using Microsoft.Extensions.DependencyInjection;
using Tand.Core;
using Tand.Core.Api;
using Tand.Core.Models;
using Xunit;

namespace Tand.Extensions.DependencyInjection.Tests
{
    public class TandServiceExtensionsTests
    {
        public const string Greeting = "Hello, World!";
        
        [Fact]
        public void RegisterService_RegisterSampleService_SuccessfullyResolve()
        {
            var calledOnEnter = false;
            var calledOnExit = false;
            var servides = new ServiceCollection();
            servides.AddTand();
            servides.AddSingleton(new TestDecorator(_ => { calledOnEnter = true;}, _ => { calledOnExit = true;}));
            servides.AddTandTransient<ISampleService, SampleServiceImpl>();
            var provider = servides.BuildServiceProvider();

            var sampleService = provider.GetService<ISampleService>();
            Assert.NotNull(sampleService);
            var result = sampleService.Greet();
            
            Assert.Equal(Greeting, result);
            Assert.True(calledOnEnter);
            Assert.True(calledOnExit);
        }
    }
    
    public class TestDecorator : ITandTarget<ISampleService>
    {
        private readonly Action<CallEnterContext<ISampleService>> _onEnterHandle;
        private readonly Action<CallLeaveContext<ISampleService>> _onLeaveHandle;

        public TestDecorator(Action<CallEnterContext<ISampleService>> onEnterHandle, Action<CallLeaveContext<ISampleService>> onLeaveHandle)
        {
            _onEnterHandle = onEnterHandle;
            _onLeaveHandle = onLeaveHandle;
        }

        public void OnEnterMethod(CallEnterContext<ISampleService> enterContext) => _onEnterHandle(enterContext);

        public void OnLeaveMethod(CallLeaveContext<ISampleService> leaveContext) => _onLeaveHandle(leaveContext);
    }

    public interface ISampleService
    {
        [Tand(typeof(TestDecorator))]
        string Greet();
    }

    public class SampleServiceImpl : ISampleService
    {
        public string Greet() => TandServiceExtensionsTests.Greeting;
    }
}