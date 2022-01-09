using System.Diagnostics;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace Dotnetty.Websocket.Demo;

    public abstract class WebsocketHandler : SimpleChannelInboundHandler<object>
    {
        bool isHandshaker = false;

        WebSocketServerHandshaker handshaker;

        protected abstract void Handle(IChannelHandlerContext ctx, WebSocketFrame frame);
        
        protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
        {
            if (this.isHandshaker)
            {
                if (msg is not WebSocketFrame frame)
                {
                    throw new InvalidOperationException("Already hand shaker please send frame.");
                }
                
                this.HandleFrame(ctx, frame);
                return;
            }
            
            if (msg is not IFullHttpRequest req)
            {
                throw new InvalidOperationException("Not ready hand shaker");
            }
            
            var wsFactory = new WebSocketServerHandshakerFactory(
                string.Empty, null, true, 5 * 1024 * 1024);
            this.handshaker = wsFactory.NewHandshaker(req);

            Task handshakerTask = null;
            if (this.handshaker == null)
            {
                handshakerTask = WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
            }
            else
            {
                handshakerTask = this.handshaker.HandshakeAsync(ctx.Channel, req);
            }

            handshakerTask.ContinueWith((ct) => this.isHandshaker = true);
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            context.DisconnectAsync();

            base.UserEventTriggered(context, evt);
        }

        void HandleFrame(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
            switch (frame)
            {
                case CloseWebSocketFrame:
                    this.handshaker.CloseAsync(ctx.Channel, (CloseWebSocketFrame)frame.Retain());
                    return;
                case PingWebSocketFrame:
                    ctx.WriteAsync(new PongWebSocketFrame((IByteBuffer)frame.Content.Retain()));
                    return;
                default:
                    this.Handle(ctx, frame);
                    break;
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }