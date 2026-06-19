using EFModels;

namespace APIsNABH
{
	public class RequestDto
	{
		public class tblApiUser
		{

			public string emailId { get; set; }
			public string mobileNumber { get; set; }
			public string orgName { get; set; }

		}

		public class AddOrginization
		{
			public string AdminEmailId { get; set; }
			public string AdminPassword { get; set; }
			public tblApiUser ordDetail {  get; set; }
			public AddOrginization()
			{
				ordDetail = new tblApiUser();
			}

		}
	}
} 
