//   Copyright 2018 yinyue200.com

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System;
using System.Collections.Generic;

namespace LottieUWP
{
    /// <summary>
    /// Contains class to hold the resulting value of an async task or an exception if it failed.
    /// Either value or exception will be non-null.
    /// </summary>
    /// <typeparam name="V">The value type that this result holds.</typeparam>
    public class LottieResult<V>
    {
        public V Value { get; }
        public Exception Exception { get; }

        public LottieResult(V value)
        {
            Value = value;
            Exception = null;
        }

        public LottieResult(Exception exception)
        {
            Exception = exception;
            Value = default(V);
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (!(o is LottieResult<V> that))
            {
                return false;
            }
            if (Value != null && Value.Equals(that.Value))
            {
                return true;
            }
            if (Exception != null && that.Exception != null)
            {
                return Exception.ToString().Equals(that.Exception.ToString());
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 512007760;
            hashCode = hashCode * -1521134295 + EqualityComparer<V>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + EqualityComparer<Exception>.Default.GetHashCode(Exception);
            return hashCode;
        }
    }
}
