// See https://aka.ms/new-console-template for more information

using DotNetty.Codecs.Http;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Dotnetty.Websocket.Demo;

var bossGroup = new DispatcherEventLoopGroup();
var workGroup = new WorkerEventLoopGroup(bossGroup);
var bootstrap = new ServerBootstrap();

const int alldleTime = 1;

bootstrap.Group(bossGroup, workGroup)
    .Channel<TcpServerChannel>()
    .Option(ChannelOption.SoBacklog, 8192)
    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
    {
        IChannelPipeline pipeline = channel.Pipeline;
        pipeline.AddFirst(new IdleStateHandler(0, 0, alldleTime));
        pipeline.AddLast(new HttpServerCodec());
        pipeline.AddLast(new HttpObjectAggregator(65536));
        pipeline.AddLast(new MyHandler());
    }));

bootstrap.BindAsync(8007);


Console.WriteLine("Hello, World!");