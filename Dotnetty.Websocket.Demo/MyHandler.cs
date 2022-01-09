// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dotnetty.Websocket.Demo;

using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;

public class MyHandler : WebsocketHandler
{
    protected override void Handle(IChannelHandlerContext ctx, WebSocketFrame frame)
    {
        ctx.WriteAndFlushAsync(frame.Retain());
    }
}