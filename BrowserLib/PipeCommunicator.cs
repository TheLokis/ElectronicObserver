using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BrowserLib
{

	/// <summary>
	/// WCFのNetNamedPipeBindingを使ったサーバ・クライアント
	/// </summary>
	public class PipeCommunicator<ClientType> where ClientType : class
	{
		private ServiceHost Server;
		private string ServerUrl;
		private ChannelFactory<ClientType> PipeFactory;
		private NetNamedPipeBinding Binding = new NetNamedPipeBinding();

		/// <summary>
		/// サーバ側オブジェクトへの通信インターフェース
		/// 通信エラーが発生するとnullになることがあるので注意
		/// </summary>
		public ClientType Proxy { get; private set; }

		/// <summary>
		/// エラーハンドラ
		/// </summary>
		public delegate void FaultedDelegate(Exception e);
		public event FaultedDelegate Faulted = delegate { };

		/// <summary>
		/// Closeされたか
		/// </summary>
		public bool Closed { get; private set; }

		/// <summary>
		/// サーバ起動もする
		/// </summary>
		public PipeCommunicator(object instance, Type type, string listenUrl, string serviceAddress)
		{
            this.Server = new ServiceHost(instance, new Uri[] { new Uri(listenUrl) });
            this.Binding.ReceiveTimeout = TimeSpan.MaxValue;
            this.Server.AddServiceEndpoint(type, this.Binding, serviceAddress);
            this.Server.Open();
		}

		/// <summary>
		/// サーバに接続
		/// </summary>
		public void Connect(string to)
		{
			if (this.Proxy == null)
			{
                this.ServerUrl = to;
				if (this.PipeFactory == null)
				{
                    this.PipeFactory = new ChannelFactory<ClientType>(this.Binding, new EndpointAddress(this.ServerUrl));
				}
                this.Proxy = this.PipeFactory.CreateChannel();
                this.Closed = false;
			}
		}

		/// <summary>
		/// 閉じる
		/// </summary>
		public void Close()
		{
			if (!this.Closed)
			{
				if (this.Proxy != null)
				{
					((IClientChannel)this.Proxy).Abort();
                    this.Proxy = null;
				}
                this.Server.Close();
                this.Closed = true;
			}
		}

        public async Task CloseAsync(object instance)
        {
            if (!this.Closed)
            {
                if (this.Proxy != null)
                {
                    ((IClientChannel)this.Proxy).Abort();
                    this.Proxy = null;
                }
                await Task.Factory.FromAsync(this.Server.BeginClose(_ => { }, instance), _ => { });

                this.Closed = true;
            }
        }

        /// <summary>
        /// 非同期でactionを実行。例外が発生したらFaultイベントが発生するので、例外が出ることはない
        /// </summary>
        public async void AsyncRemoteRun(Action action)
		{
			try
			{
				// リトライループ
				for (int i = 0; i < 2; ++i)
				{
					try
					{
						if (this.Proxy == null) return;
						await Task.Run(action);
						return;
					}
					catch (CommunicationException cex)
					{
						((IClientChannel)this.Proxy).Abort();
                        this.Proxy = null;
						if (i >= 1)
						{
							// これ以上リトライしない
							Faulted(cex);
							break;
						}
                        this.Connect(this.ServerUrl);
					}
				}
			}
			catch (Exception ex)
			{
				Faulted(ex);
			}
		}
	}

}
