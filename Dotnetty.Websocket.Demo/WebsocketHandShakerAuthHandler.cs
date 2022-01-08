using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;

namespace Dotnetty.Websocket.Demo;

public class WebsocketHandShakerAuthHandler : SimpleChannelInboundHandler<object>
{
    bool handShakered = false;

    IWebsocketHandShakerAuthorizer authorizer;

    public WebsocketHandShakerAuthHandler() : this(new DefaultWebsocketHandShakerAuthorizer()){}

    public WebsocketHandShakerAuthHandler(IWebsocketHandShakerAuthorizer authorizer)
    {
        this.authorizer = authorizer;
    }
        
    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
    {
        if (this.handShakered)
        {
            ctx.FireChannelRead(msg);
            return;
        }
                
        if (msg is WebSocketFrame frame || msg is not IFullHttpRequest request)
        {
            throw new InvalidOperationException($"not yet hand shaker.");
        }

        if (!authorizer.Auth(request))
        {
            throw new InvalidOperationException($"unauthorized connecting.");
        }
        
        var wsFactory = new WebSocketServerHandshakerFactory(
            "ws", null, true, 5 * 1024 * 1024);
        this.handshaker = wsFactory.NewHandshaker(req);
        if (this.handshaker == null)
        {
            WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
        }
        else
        {
            this.handshaker.HandshakeAsync(ctx.Channel, req);
        }
        handShakered = true;
    }
}