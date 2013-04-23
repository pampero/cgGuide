using System;
using System.IO;
using ServiceStack.Service;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;

namespace RedisWebServices.Tests
{
	public class TestServiceClient
		: IServiceClient
	{
		private readonly AppHostBase appHostBase;

		public TestServiceClient(AppHostBase appHostBase)
		{
			this.appHostBase = appHostBase;
		}

		public TResponse Send<TResponse>(object request)
		{
			return (TResponse) appHostBase.ExecuteService(request);
		}

	    public TResponse Send<TResponse>(IReturn<TResponse> request)
	    {
	        throw new NotImplementedException();
	    }

	    public void Send(IReturnVoid request)
	    {
	        throw new NotImplementedException();
	    }

	    public TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, FileInfo fileToUpload, string mimeType)
		{
			throw new NotImplementedException();
		}

	    public TResponse PostFile<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileName, string mimeType)
	    {
	        throw new NotImplementedException();
	    }

	    public TResponse PostFileWithRequest<TResponse>(string relativeOrAbsoluteUrl, FileInfo fileToUpload, object request)
	    {
	        throw new NotImplementedException();
	    }

	    public TResponse PostFileWithRequest<TResponse>(string relativeOrAbsoluteUrl, Stream fileToUpload, string fileName, object request)
	    {
	        throw new NotImplementedException();
	    }

	    public void SendOneWay(object request)
		{
			appHostBase.ExecuteService(request);
		}

		public void SendOneWay(string relativeOrAbsoluteUrl, object request)
		{
			appHostBase.ExecuteService(request);
		}

		public void SetCredentials(string userName, string password)
		{
			throw new NotImplementedException();
		}

		public void GetAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
		{
			throw new NotImplementedException();
		}

		public void DeleteAsync<TResponse>(string relativeOrAbsoluteUrl, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
		{
			throw new NotImplementedException();
		}

		public void PostAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess,
			Action<TResponse, Exception> onError)
		{
			throw new NotImplementedException();
		}

		public void PutAsync<TResponse>(string relativeOrAbsoluteUrl, object request, Action<TResponse> onSuccess, 
			Action<TResponse,Exception> onError)
		{
			throw new NotImplementedException();
		}

		public void SendAsync<TResponse>(object request, Action<TResponse> onSuccess, Action<TResponse, Exception> onError)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}
	}
}