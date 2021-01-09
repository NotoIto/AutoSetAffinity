using System;
using Optional;

namespace Domain
{
    public class DomainDefinedError : Exception {
        public DomainDefinedError(string message, Exception e) : base(message, e) { }
    }

    public class SystemError : DomainDefinedError {
        public SystemError(string message, Exception e) : base($"SystemError({message})", e) { }
    }

    public static class Try2OptionSystemError
    {
        public static Option<TResult, DomainDefinedError> ToOptionSystemError<TResult>(this Func<TResult> tryFunction, string message)
        {
            try
            {
                var result = tryFunction();
                return Option.Some<TResult, DomainDefinedError>(result);
            }
            catch (Exception e)
            {
                return Option.None<TResult, DomainDefinedError>(new SystemError(message, e));
            }
        }
    }
}
