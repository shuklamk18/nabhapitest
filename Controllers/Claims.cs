using APIsNABH.Attributes;
using APIsNABH.Models;
using EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.MicrosoftExtensions;
using System.Data.Entity;
using System.Security.Claims;
namespace APIsNABH.Controllers
{
	[EnableRateLimiting("ApiPolicy")]
	[Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
	public class Claims : ControllerBase
	{
		[Permission("AddClaim")]
		[HttpPost]
		public async Task<IActionResult> AddClaim(Models.AssessmentDataForOtherPortal i)
		{
			ApiResponce<object> resp = new ApiResponce<object>();
			
			int claimId = 0;
			var cc = User.Claims;
			try
			{
				if (!ModelState.IsValid)
				{
					resp.Success = false;
					resp.Data = ModelState;
					resp.Message = "Invalid Input";

					return UnprocessableEntity(resp);
				}
				nabhEntities _db = new nabhEntities();

				var Assessor = await _db.AssessorClaimLogins.FirstOrDefaultAsync(x => x.panNo == i.PanNumber.Trim());

				if (Assessor != null)
				{

					int?[] notAlowedState = [1, 2];
					var claims = await _db.AssessorClaims.FirstOrDefaultAsync(x=>x.RefNo.Trim() == i.RefNo.Trim() && x.AssessorId==Assessor.id && notAlowedState.Contains(x.StatusId));
					
					if (claims == null)
					{
						AssessorClaim claim = new AssessorClaim();
						claim.AssessorId = Assessor.id;

						claim.StatusId = 1;
						claim.RefNo = i.RefNo.Trim();
						claim.HCOName = i.HCOName.Trim();
						claim.reviewCycle = i.ReviewCycles;
						claim.userRole = i.AssessorType.Trim();
						claim.fromDate = i.FromDate;
						claim.toDate = i.ToDate;
						claim.Address = i.Address.Trim();
						claim.assignedTo = 6;
						claim.Remark = "";

						claim.AllowEdit = false ;

						claim.CreatedOn = DateTime.Now;
						claim.ModifiedOn = DateTime.Now;






						var State = await _db.STATES.FirstOrDefaultAsync(x => x.STATE1.Trim() == i.HCOState.Trim());

						if (State != null)
						{
							var City = await _db.Cities.FirstOrDefaultAsync(x => x.StateID == State.ID && x.CityName.Trim() == i.HCOCity.Trim());
							if (City != null)
							{
									claim.cityId = City.ID;
							}
							else
							{
								resp.Success = false;
								resp.Message = "CityName " + i.HCOCity + " does not exists!";
								return UnprocessableEntity(resp);
							}
						}
						else
						{
							resp.Success = false;
							resp.Message = "HCOState " + i.HCOState + " does not exists!";
							return UnprocessableEntity(resp);
						}


						var Program = await _db.ClaimProgrammes.FirstOrDefaultAsync(x => x.name == i.ProgramName.Trim());

						if (Program != null)
						{
							claim.ApplicationTypeId = Program.id.ToString();
						}
						else
						{
							resp.Success = false;
							resp.Message = i.ProgramName + " does not exists !";
							return UnprocessableEntity(resp);
						}

							var ApplicationType = await _db.ClaimTasks.FirstOrDefaultAsync(x => x.name == i.ApplicationType.Trim() && x.programmeId== Program.id);

						if (ApplicationType != null)
						{
								claim.AssessmentTypeId = ApplicationType.id;
						}
						else
						{
							resp.Success = false;
							resp.Message = i.ApplicationType + " does not exists !";
							return UnprocessableEntity(resp);
						}

						var Assessment = await _db.ClaimTaskActivities.FirstOrDefaultAsync(x => x.name == i.AssessmentType && x.TaskId== claim.AssessmentTypeId);

						if (Assessment != null)
						{
							claim.ActivityId = Assessment.id;
						}
						else
						{
							resp.Success = false;
							resp.Message = i.AssessmentType + " does not exists !";
							return UnprocessableEntity(resp); 
						}



						//AddAssessorBankDetail
						claim.assessorAccountName = Assessor.assessorAccountName;
						claim.bankName = Assessor.bankName;
						claim.bankBranch = Assessor.bankBranch;
						claim.bankIfsc = Assessor.bankIfsc;
						claim.bankAccountNo = Assessor.bankAccountNo;
						claim.panNo = Assessor.panNo;



						_db.AssessorClaims.Add(claim);
						await _db.SaveChangesAsync();
						resp.Success = true;
						resp.Message = "Successfully added the claim!";
						resp.Data = new { ClaimId = claim.ClaimId };

						return StatusCode(StatusCodes.Status201Created,resp);
 
					}
					else
					{
						resp.Success = false;
						resp.Message = "Claim is in-process!";
						return Conflict(resp);

					}
				}
				else
				{
					resp.Success = false;
					resp.Message = "Assessor not found with provided pan number!";
					return NotFound(resp);
 
				}

			}
			catch (Exception ex)
			{
				resp.Success = false;
				resp.Message = "Internal Server Error!";
				return StatusCode(StatusCodes.Status500InternalServerError, resp);
			}

		}

		[HttpPost]
        [Permission("AddClaim")]
        public async Task<IActionResult> UpdateClaim(Models.AssessmentDataForOtherPortal i)
		{
			ApiResponce<object> resp = new ApiResponce<object>();

			int claimId = 0;

			try
			{
				if (!ModelState.IsValid)
				{
					resp.Success = false;
					resp.Data = ModelState;
					resp.Message = "Invalid Input";

					return UnprocessableEntity(resp);
				}
				nabhEntities _db = new nabhEntities();

				var Assessor = await _db.AssessorClaimLogins.FirstOrDefaultAsync(x => x.panNo == i.PanNumber.Trim());

				if (Assessor != null)
				{

					int?[] notAlowedState = [1, 2];
					var claim = await _db.AssessorClaims.FirstOrDefaultAsync(x => x.ClaimId==i.ClaimId);

					if (claim != null)
					{
						claimId = claim.ClaimId;
						//claim.StatusId = 1;
						claim.RefNo = i.RefNo.Trim();
						claim.HCOName = i.HCOName.Trim();
						claim.reviewCycle = i.ReviewCycles;
						claim.userRole = i.AssessorType.Trim();
						claim.fromDate = i.FromDate;
						claim.toDate = i.ToDate;
						claim.Address = i.Address.Trim();
						//claim.assignedTo = 6;
						//claim.Remark = "";

						claim.AllowEdit = false;

						//claim.CreatedOn = DateTime.Now;
						claim.ModifiedOn = DateTime.Now;






						var State = await _db.STATES.FirstOrDefaultAsync(x => x.STATE1.Trim() == i.HCOState.Trim());

						if (State != null)
						{
							var City = await _db.Cities.FirstOrDefaultAsync(x => x.StateID == State.ID && x.CityName.Trim() == i.HCOCity.Trim());
							if (City != null)
							{
								claim.cityId = City.ID;
							}
							else
							{
								resp.Success = false;
								resp.Message = "CityName " + i.HCOCity + " does not exists!";
								return UnprocessableEntity(resp);
							}
						}
						else
						{
							resp.Success = false;
							resp.Message = "HCOState " + i.HCOState + " does not exists!";
							return UnprocessableEntity(resp);
 						}


						var Program = await _db.ClaimProgrammes.FirstOrDefaultAsync(x => x.name == i.ProgramName.Trim());

						if (Program != null)
						{
							claim.ApplicationTypeId = Program.id.ToString();
						}
						else
						{
							resp.Success = false;
							resp.Message = i.ProgramName + " does not exists !";
							return UnprocessableEntity(resp);
 						}

						var ApplicationType = await _db.ClaimTasks.FirstOrDefaultAsync(x => x.name == i.ApplicationType.Trim() && x.programmeId == Program.id);

						if (ApplicationType != null)
						{
							claim.AssessmentTypeId = ApplicationType.id;
						}
						else
						{
							resp.Success = false;
							resp.Message = i.ApplicationType + " does not exists !";
							return UnprocessableEntity(resp);
 						}

						var Assessment = await _db.ClaimTaskActivities.FirstOrDefaultAsync(x => x.name == i.AssessmentType && x.TaskId == claim.AssessmentTypeId);

						if (Assessment != null)
						{
							claim.ActivityId = Assessment.id;
						}
						else
						{
							resp.Success = false;
							resp.Message = i.AssessmentType + " does not exists !";
							return UnprocessableEntity(resp); 
						}



						//AddAssessorBankDetail
						claim.assessorAccountName = Assessor.assessorAccountName;
						claim.bankName = Assessor.bankName;
						claim.bankBranch = Assessor.bankBranch;
						claim.bankIfsc = Assessor.bankIfsc;
						claim.bankAccountNo = Assessor.bankAccountNo;
						claim.panNo = Assessor.panNo;



						//_db.AssessorClaims.Add(claim);
						await _db.SaveChangesAsync();
						resp.Success = true;
						resp.Message = "Successfully updated the claim!";
						resp.Data = new { ClaimId = claimId };
						return StatusCode(StatusCodes.Status201Created,resp);

					}
					else
					{
						resp.Success = false;
						resp.Message = "Claim ID " + claimId + " not found!";
						return NotFound(resp);
 
					}
				}
				else
				{
					resp.Success = false;
					resp.Message =  "Assessor not found with provided pan number!";
					return NotFound(resp);
 
				}

			}
			catch (Exception ex)
			{
				resp.Success = false;
				resp.Message = "Internal Server Error!";

				return StatusCode(StatusCodes.Status500InternalServerError, resp);
			}

		}

		[HttpGet]
        [Permission("ViewClaim")]
        public async Task<IActionResult> GetClaimDetail(int claimId)
		{
			ApiResponce<AssessorClaim> resp = new ApiResponce<AssessorClaim>();


			try
			{
				nabhEntities _db = new nabhEntities();
				_db.Configuration.LazyLoadingEnabled = false;

				AssessorClaim claim = await _db.AssessorClaims.FirstOrDefaultAsync(x => x.ClaimId == claimId);
				if (claim != null)
				{
					resp.Success = true;
					resp.Message = "Ok!";
					resp.Data = claim;
					return Ok(resp);
				}
				else
				{
					resp.Success = false;
					resp.Message = "No claim found!";
 					return NotFound(resp);
				}
			}
			catch (Exception ex)
			{
				resp.Success = false;
				resp.Message = "Internal Server Error!";
				return StatusCode(StatusCodes.Status500InternalServerError,resp);
			}

		}

        [Permission("ViewClaim")]
        [HttpGet]
		public async Task<IActionResult> GetClaimListByAssessorIdOrPanNumber(int? AssessorId,string? PanNo)
		{

			ApiResponce<List<AssessorClaim>> resp = new ApiResponce<List<AssessorClaim>>();

			try
			{
				nabhEntities _db = new nabhEntities();
				_db.Configuration.ProxyCreationEnabled = false;
				_db.Configuration.LazyLoadingEnabled = false;

				var Assessor = await _db.AssessorClaimLogins.FirstOrDefaultAsync(x => x.panNo == PanNo.Trim() || x.id== AssessorId);

				if (Assessor != null)
				{
					var claims = await _db.AssessorClaims
						.Include(x => x.AssessorBills)
						.Where(x => x.AssessorId == Assessor.id)
						.ToListAsync();

					if (claims.Any()) { 
						resp.Success = true;
						resp.Data = claims;
						resp.Message = "Ok!";
						return Ok(resp);
					}
					else
					{
						resp.Success = false;
						resp.Message = "No claim found!";
						return NotFound(resp);
					}
					
				}
				else
				{
					resp.Success = false;
					resp.Message = "No assessor found!";
					return NotFound(resp);
				}

				
			}
			catch (Exception ex)
			{
				resp.Success = false;
				resp.Message = "Internal Server Error!";
 				return StatusCode(StatusCodes.Status500InternalServerError, resp);
			}

		}
	}
}
