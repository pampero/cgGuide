using ServiceStack.ServiceInterface.Auth;

namespace Model.Model.entities
{
    //A customizeable typed UserSession that can be extended with your own properties
    //To access ServiceStack's Session, Cache, etc from MVC Controllers inherit from ControllerBase<CustomUserSession>
    public class CustomUserSession : AuthUserSession
    {
        public string CustomProperty { get; set; }
    }
}
