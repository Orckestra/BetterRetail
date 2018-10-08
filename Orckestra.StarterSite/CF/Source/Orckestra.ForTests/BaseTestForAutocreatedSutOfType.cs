﻿using Moq;
using Moq.AutoMock;

namespace Orckestra.ForTests
{
    public class BaseTestForAutocreatedSutOfType<TSystemUnderTest> where TSystemUnderTest : class
    {
        protected AutoMocker Container;

        protected BaseTestForAutocreatedSutOfType()
        {
            Container = new AutoMocker();
        }

       protected Mock<TInterface> Dependency<TInterface>() where TInterface : class
        {
            var dependency = Container.GetMock<TInterface>();
            try
            {
                
            }
            catch
            {

            }
            return dependency;
        }

 
    }
}