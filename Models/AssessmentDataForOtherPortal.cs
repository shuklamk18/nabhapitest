using EFModels;
using System.ComponentModel.DataAnnotations;

namespace APIsNABH.Models
{
	public partial class AssessmentDataForOtherPortal
	{


		public int ClaimId { get; set; }

		public int AssignmentID { get; set; }
		[Required]
		public string RefNo { get; set; }
		[Required]
		[MaxLength(300,ErrorMessage = "HCOName cannot exceed 300 characters.")]
		public string HCOName { get; set; }

		[Required]
		public int ReviewCycles { get; set; }

		[Required]
		public string ProgramName { get; set; }
		[Required]
		public string ApplicationType { get; set; }
		[Required]
		public string AssessmentType { get; set; }
		[Required]
		public string AssessorType { get; set; }

		[Required]
		public string HCOCountry { get; set; }
		[Required]
		public string HCOState { get; set; }

		[Required]
		public string HCOCity { get; set; }

		[Required]
		public string Address { get; set; }
		[Required]
		[RegularExpression(@"^(Pending|Completed)$", ErrorMessage ="Invalid Status" )]
		public string Status { get; set; }
		[Required]
		[RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$",
			ErrorMessage = "Invalid PAN Number.")]
		public string PanNumber { get; set; }

		[Required]
		public DateTime FromDate { get; set; }

		[Required]
		public DateTime ToDate { get; set; }

	}

	public class ApiResponce<T>
	{
		public bool Success { get; set; } = true;
		public string Message { get; set; } = string.Empty;
		public T? Data { get; set; }

	}

}
