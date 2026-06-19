using APIsNABH.Attributes;
using APIsNABH.dto;
using EFModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;

namespace APIsNABH.Controllers
{
    [EnableRateLimiting("ApiPolicy")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EventAndTrainingUserController : ControllerBase
    {
        TripleDESStringEncryptor objtdesse = new TripleDESStringEncryptor();
        nabhEntities _context;
        public EventAndTrainingUserController(nabhEntities nabhEntities) { 
            _context = nabhEntities;
        }

        [Authorize]
        [Permission("UserBypass")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] DtoCreateUserRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request"
                    });
                }

                TripleDESStringEncryptor objtdesse = new TripleDESStringEncryptor();
                var existingUser = _context.tblUsers.FirstOrDefault(x => x.Username == request.Email);
                if (existingUser != null)
                {
                    string existingToken = WebUtility.UrlEncode(objtdesse.EncryptString(existingUser.Userid.ToString()));
                    return Conflict(new
                    {
                        success = true,
                        message = "Email already exists",
                        token = existingToken
                    });

                }

                tblUser user = new tblUser
                {
                    Username = request.Email.Trim(),
                    Password = objtdesse.EncryptString(request.Password),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName?.Trim(),
                    RoleID = 20,
                    Active = true,
                    Phone = request.Phone ?? "",
                    Extension = 0,
                    Mobile = request.Mobile ?? "",
                    EmployeeNumber = 0,
                    PartnerID = null,
                    OrganisationID = 0,
                    CreatedBy = request.FirstName.Trim(),
                    CreatedDate = DateTime.Now,
                    ModifiedBy = null,
                    ModifiedDate = null,
                    TechAdmin = false,
                    Location = "",
                    PrimaryCompetence = "",
                    SecondaryCompetence = "",
                    PreferredAreaofCalling = "",
                    Experience = "",
                    AgewithCompany = "",
                    Residence = "",
                    ReportTo = 0,
                    UserGroup = "",
                    ExpDate = null,
                    Designation = request.Designation ?? "",
                    Department = request.Department ?? "",
                    Category = "",
                    Speciality = "",
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    OrganisationName = request.OrganizationName,
                    OthersSpeciality = "",
                    DOB = null,
                    PinCode = ""
                };

                string passwordDecrypt = objtdesse.DecryptString(user.Password);
                _context.tblUsers.Add(user);
                await _context.SaveChangesAsync();

                VerifiedDocNumber doc = new VerifiedDocNumber
                {
                    Userid = user.Userid,
                    PanNumber = request.PanNumber?.ToUpper(),
                    TanNumber = request.TanNumber?.ToUpper(),
                    GSTNumber = request.GstNumber?.ToUpper(),
                    IsGstAvailable = request.IsGstAvailable
                };

                _context.VerifiedDocNumbers.Add(doc);
                await _context.SaveChangesAsync();

                string encryptedUserId = objtdesse.EncryptString(user.Userid.ToString());
                string token = WebUtility.UrlEncode(encryptedUserId);

                return Ok(new
                {
                    success = true,
                    message = "User created successfully",
                    token = token
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
