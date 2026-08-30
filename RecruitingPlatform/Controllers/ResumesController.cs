using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecruitingPlatform.Const.Resumes;
using RecruitingPlatform.DTOs.Resumes;
using RecruitingPlatform.Enums;
using RecruitingPlatform.Services.Resumes;
using RecruitingPlatform.Services.Skills;
using RecruitingPlatform.Services.Specialties;
using RecruitingPlatform.ViewModels.Resumes;
using System.Security.Claims;

namespace RecruitingPlatform.Controllers
{
    public class ResumesController(
        IGetResumeByIdService _getResumeByIdService,
        IGetResumesWithFiltersService _getResumesWithFiltersService,
        IGetAllSpecialtiesService _getAllSpecialtiesService,
        IGetAllSkillsService _getAllSkillsService,
        ICreateResumeService _createResumeService,
        IEditResumeService _editResumeService,
        IDeleteResumeService _deleteResumeService,
        ILogger<ResumesController> _logger)
        : Controller
    {
        [Authorize(Roles = nameof(PossibleUserRole.Employer) + "," + nameof(PossibleUserRole.Admin))]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ResumeFiltersDto filters)
        {
            if (filters.Page < 1) filters.Page = 1;

            _logger.LogInformation("Fetching resumes with filters. Page: {Page}", filters.Page);
            var result = await _getResumesWithFiltersService.ExecuteAsync(filters);
            var specialties = await _getAllSpecialtiesService.ExecuteAsync();

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.Filters = filters;
            ViewBag.Specialties = specialties;

            return View(result.Items);
        }

        [Authorize(Roles = nameof(PossibleUserRole.Employer) + "," + nameof(PossibleUserRole.Admin) + "," + nameof(PossibleUserRole.JobSeeker))]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var resume = await _getResumeByIdService.ExecuteAsync(id);
            if (resume == null)
            {
                _logger.LogWarning("Requested details for non-existent resume ID: {ResumeId}", id);
                return View("ResumeNotFound");
            }
            return View(resume);
        }

        [Authorize(Roles = nameof(PossibleUserRole.JobSeeker))]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var specialties = await _getAllSpecialtiesService.ExecuteAsync();
            var allSkills = await _getAllSkillsService.ExecuteAsync();

            var groupedSkills = allSkills
                .GroupBy(s => s.SkillType?.Name ?? "Інше")
                .ToDictionary(g => g.Key, g => g.ToList());

            var viewModel = new CreateResumeViewModel
            {
                Specialties = specialties,
                GroupedSkills = groupedSkills,
                FormData = new CreateResumeDto()
            };

            return View(viewModel);
        }

        [Authorize(Roles = nameof(PossibleUserRole.JobSeeker))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateResumeDto formData)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int jobSeekerId))
            {
                _logger.LogWarning("Failed to parse job seeker ID from claims during resume creation.");
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state submitted during resume creation for user {JobSeekerId}.", jobSeekerId);
                var allSkills = await _getAllSkillsService.ExecuteAsync();
                var viewModel = new CreateResumeViewModel
                {
                    Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                    GroupedSkills = allSkills
                        .GroupBy(s => s.SkillType?.Name ?? "Інше")
                        .ToDictionary(g => g.Key, g => g.ToList()),
                    FormData = formData
                };

                return View(viewModel);
            }

            var newResumeId = await _createResumeService.ExecuteAsync(formData, jobSeekerId);
            _logger.LogInformation("Job seeker {JobSeekerId} successfully created resume {ResumeId}.", jobSeekerId, newResumeId);

            TempData[ResumesConstants.SuccessMessageTempDataKey] = ResumesConstants.ResumeCreatedSuccessMessage;

            return RedirectToAction(nameof(Details), new { id = newResumeId });
        }

        [Authorize(Roles = $"{nameof(PossibleUserRole.JobSeeker)},{nameof(PossibleUserRole.Admin)}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

            bool isAdmin = User.IsInRole(nameof(PossibleUserRole.Admin));

            var dto = await _editResumeService.GetForEditAsync(id, jobSeekerId, isAdmin);
            if (dto == null)
            {
                _logger.LogWarning("Resume {ResumeId} not found or user {JobSeekerId} lacks edit permissions.", id, jobSeekerId);
                return NotFound(ResumesConstants.ResumeNotFoundErrorMessage);
            }

            var allSkills = await _getAllSkillsService.ExecuteAsync();
            var viewModel = new EditResumeViewModel
            {
                Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                GroupedSkills = allSkills.GroupBy(s => s.SkillType?.Name ?? "Інше").ToDictionary(g => g.Key, g => g.ToList()),
                FormData = dto
            };

            return View(viewModel);
        }

        [Authorize(Roles = $"{nameof(PossibleUserRole.JobSeeker)},{nameof(PossibleUserRole.Admin)}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditResumeDto formData)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state submitted during resume edit for resume ID: {ResumeId}", formData.Id);
                var allSkills = await _getAllSkillsService.ExecuteAsync();
                var viewModel = new EditResumeViewModel
                {
                    Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                    GroupedSkills = allSkills.GroupBy(s => s.SkillType?.Name ?? "Інше").ToDictionary(g => g.Key, g => g.ToList()),
                    FormData = formData
                };
                return View(viewModel);
            }

            bool isAdmin = User.IsInRole(nameof(PossibleUserRole.Admin));
            bool isSuccess = await _editResumeService.UpdateAsync(jobSeekerId, formData, isAdmin);

            if (!isSuccess)
            {
                _logger.LogError("Failed to update resume {ResumeId} for user {JobSeekerId}.", formData.Id, jobSeekerId);
                ModelState.AddModelError(string.Empty, ResumesConstants.ResumeUpdateFailedMessage);
                var allSkills = await _getAllSkillsService.ExecuteAsync();
                var viewModel = new EditResumeViewModel
                {
                    Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                    GroupedSkills = allSkills.GroupBy(s => s.SkillType?.Name ?? "Інше").ToDictionary(g => g.Key, g => g.ToList()),
                    FormData = formData
                };
                return View(viewModel);
            }

            _logger.LogInformation("Successfully updated resume {ResumeId}.", formData.Id);
            TempData[ResumesConstants.SuccessMessageTempDataKey] = ResumesConstants.ResumeUpdatedSuccessMessage;
            return RedirectToAction(nameof(Details), new { id = formData.Id });
        }

        [Authorize(Roles = $"{nameof(PossibleUserRole.JobSeeker)},{nameof(PossibleUserRole.Admin)}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

            bool isAdmin = User.IsInRole(nameof(PossibleUserRole.Admin));
            bool isSuccess = await _deleteResumeService.DeleteAsync(id, jobSeekerId, isAdmin);

            if (!isSuccess)
            {
                _logger.LogError("Failed to delete resume {ResumeId} by user {UserId}. IsAdmin: {IsAdmin}", id, jobSeekerId, isAdmin);
                TempData[ResumesConstants.ErrorMessageTempDataKey] = ResumesConstants.ResumeDeleteFailedMessage;
                return RedirectToAction(nameof(Details), new { id = id });
            }

            _logger.LogInformation("Successfully deleted resume {ResumeId}.", id);
            TempData[ResumesConstants.SuccessMessageTempDataKey] = ResumesConstants.ResumeDeletedSuccessMessage;

            if (isAdmin)
            {
                return RedirectToAction("Index", "Admin", new { tab = "resumes" });
            }

            return RedirectToAction("Index", "Profile");
        }
    }
}