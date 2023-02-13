using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchDarkly.InitializeTimeout.Demo
{
    public class AsyncResult<T> where T : class
    {
        public T? Result { get; set; }

        public bool Success => Result != null && Error == null;

        public Exception? Error { get; set; }
    }
}
