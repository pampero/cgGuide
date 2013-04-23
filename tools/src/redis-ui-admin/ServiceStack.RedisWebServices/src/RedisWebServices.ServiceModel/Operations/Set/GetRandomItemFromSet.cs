﻿using System.Runtime.Serialization;
using ServiceStack.DesignPatterns.Model;
using ServiceStack.ServiceInterface.ServiceModel;

namespace RedisWebServices.ServiceModel.Operations.Set
{
	[DataContract]
	public class GetRandomItemFromSet
		: IHasStringId
	{
		[DataMember] 
		public string Id { get; set; }
	}

	[DataContract]
	public class GetRandomItemFromSetResponse
	{
		public GetRandomItemFromSetResponse()
		{
			this.ResponseStatus = new ResponseStatus();
		}

		[DataMember] 
		public string Item { get; set; }

		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}
}