using Model.Model.entities;
using ServiceStack.Logging;
using ServiceStack.Mvc;

namespace Common.Base
{
    public class BaseController : ServiceStackController<CustomUserSession>
    {
        public ILog Logger { get; set; }
    }
}
