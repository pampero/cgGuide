﻿using System.Runtime.Serialization;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.ServiceInterface.ServiceModel;

namespace RedisWebServices.ServiceModel.Operations.List
{
	[DataContract]
	public class PushItemToList
		: IHasStringId
	{
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Item { get; set; }
	}

	[DataContract]
	public class PushItemToListResponse
	{
		public PushItemToListResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}
}