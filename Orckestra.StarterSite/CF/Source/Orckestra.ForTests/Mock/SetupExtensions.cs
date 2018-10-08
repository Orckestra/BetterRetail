using System;
using System.Threading.Tasks;
using Moq.Language;
using Moq.Language.Flow;

namespace Orckestra.ForTests.Mock
{
    public static class SetupExtensions
    {
        public static IReturnsResult<TMock> ReturnsTask<TMock, TResult>(
            this IReturns<TMock, Task<TResult>> setup) where TMock : class
        {
            return setup.Returns(() => Task.FromResult(default(TResult)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, TResult>(
            this IReturns<TMock, Task<TResult>> setup, TResult value) where TMock : class
        {
            return setup.Returns(() => Task.FromResult(value));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, TResult>(
            this IReturns<TMock, Task<TResult>> setup, Func<TResult> func) where TMock : class
        {
            return setup.Returns(Task.Factory.StartNew(func));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, T, TResult>(
            this IReturns<TMock, Task<TResult>> setup, Func<T, TResult> func) where TMock : class
        {
            return setup.Returns<T>(arg => Task.Factory.StartNew(() => func(arg)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, T1, T2, TResult>(
            this IReturns<TMock, Task<TResult>> setup, Func<T1, T2, TResult> func) where TMock : class
        {
            return setup.Returns<T1, T2>((arg1, arg2) => Task.Factory.StartNew(() => func(arg1, arg2)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, T1, T2, T3, TResult>(
            this IReturns<TMock, Task<TResult>> setup, Func<T1, T2, T3, TResult> func) where TMock : class
        {
            return setup.Returns<T1, T2, T3>((arg1, arg2, arg3) => Task.Factory.StartNew(() => func(arg1, arg2, arg3)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, T1, T2, T3, T4, TResult>(
            this IReturns<TMock, Task<TResult>> setup, Func<T1, T2, T3, T4, TResult> func) where TMock : class
        {
            return setup.Returns<T1, T2, T3, T4>((arg1, arg2, arg3, arg4) => Task.Factory.StartNew(() => func(arg1, arg2, arg3, arg4)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock>(this IReturns<TMock, Task> setup, Action action) where TMock : class
        {
            return setup.Returns(Task.Factory.StartNew(action));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, T>(this IReturns<TMock, Task> setup, Action<T> action) where TMock : class
        {
            return setup.Returns<T>(arg => Task.Factory.StartNew(() => action(arg)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, T1, T2>(this IReturns<TMock, Task> setup, Action<T1, T2> action) where TMock : class
        {
            return setup.Returns<T1, T2>((arg1, arg2) => Task.Factory.StartNew(() => action(arg1, arg2)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, T1, T2, T3>(this IReturns<TMock, Task> setup, Action<T1, T2, T3> action) where TMock : class
        {
            return setup.Returns<T1, T2, T3>((arg1, arg2, arg3) => Task.Factory.StartNew(() => action(arg1, arg2, arg3)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock, T1, T2, T3, T4>(this IReturns<TMock, Task> setup, Action<T1, T2, T3, T4> action) where TMock : class
        {
            return setup.Returns<T1, T2, T3, T4>((arg1, arg2, arg3, arg4) => Task.Factory.StartNew(() => action(arg1, arg2, arg3, arg4)));
        }

        public static IReturnsResult<TMock> ReturnsTask<TMock>(this IReturns<TMock, Task> setup) where TMock : class
        {
            return setup.Returns(Task.FromResult(true));
        }
    }
}
