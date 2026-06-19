namespace APIsNABH
{
	public class ApiKeyGenerator
	{
		public static (string apiKey, string apiSecret, string secretHash)
			Generate()
		{
			var apiKey =
				$"NABH_{Guid.NewGuid():N}";

			var apiSecret =
				$"{Guid.NewGuid():N}{Guid.NewGuid():N}";

			var secretHash = BCrypt.Net.BCrypt.HashPassword(apiSecret);

			return (apiKey, apiSecret, secretHash);
		}
	}
}
