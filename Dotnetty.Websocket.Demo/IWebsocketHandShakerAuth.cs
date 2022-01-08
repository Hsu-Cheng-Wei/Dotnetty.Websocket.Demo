using DotNetty.Codecs.Http;

namespace Dotnetty.Websocket.Demo;

public interface IWebsocketHandShakerAuthorizer
{
    bool Auth(IFullHttpRequest request);
}

public class DefaultWebsocketHandShakerAuthorizer : IWebsocketHandShakerAuthorizer
{
    public bool Auth(IFullHttpRequest request)
    {
        return true;
    }
}