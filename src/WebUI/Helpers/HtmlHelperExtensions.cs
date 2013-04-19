using System;
using System.Web.Mvc;

namespace Framework.Helpers
{
    public static class HtmlHelperExtensions {
        private static readonly Random rnd = new Random();

        public static int RandomNumber(this HtmlHelper helper) {
            return rnd.Next();
        }
    }
}