using System;
using System.Text.Json.Serialization;

namespace Voting2020.Core
{
	public sealed class EncryptedVote
	{

		public EncryptedVote()
		{

		}

		public EncryptedVote(string message,string nonce,string publicKey)
		{
			Message = message;
			Nonce = nonce;
			PublicKey = publicKey;
		}


		[JsonPropertyName("message")]
		public string Message { get; set; }
		
		[JsonPropertyName("nonce")]
		public string Nonce { get; set; }

		[JsonPropertyName("public_key")]
		public string PublicKey { get; set; }
	}
}
