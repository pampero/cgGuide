﻿using System;

namespace Framework.Helpers
{
    public static class StringExtensions {
        public static bool NotNullAnd(this string s, Func<string, bool> f) {
            return s != null && f(s);
        }
    }
}