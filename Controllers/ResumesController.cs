using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        IDeleteResumeService _deleteResumeService)
        : Controller
    {
        [Authorize(Roles = nameof(PossibleUserRole.Employer) + "," + nameof(PossibleUserRole.Admin))]
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ResumeFiltersDto filters)
        {
            if (filters.Page < 1) filters.Page = 1;

            var result = await _getResumesWithFiltersService.ExecuteAsync(filters);

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.Filters = filters;

            return View(result.Items);
        }

        [Authorize(Roles = nameof(PossibleUserRole.Employer) + "," + nameof(PossibleUserRole.Admin) + "," + nameof(PossibleUserRole.JobSeeker))]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var resume = await _getResumeByIdService.ExecuteAsync(id);
            if (resume == null) return View("ResumeNotFound");
            return View(resume);
        }

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
            if (!ModelState.IsValid)
            {
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

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

            var newResumeId = await _createResumeService.ExecuteAsync(formData, jobSeekerId);

            TempData[ResumesConstants.SuccessMessageTempDataKey] = ResumesConstants.ResumeCreatedSuccessMessage;

            return RedirectToAction(nameof(Details), new { id = newResumeId });
        }

        [Authorize(Roles = nameof(PossibleUserRole.JobSeeker))]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

            var dto = await _editResumeService.GetForEditAsync(id, jobSeekerId);
            if (dto == null) return NotFound(ResumesConstants.ResumeNotFoundErrorMessage);

            var allSkills = await _getAllSkillsService.ExecuteAsync();
            var viewModel = new EditResumeViewModel
            {
                Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                GroupedSkills = allSkills.GroupBy(s => s.SkillType?.Name ?? "Інше").ToDictionary(g => g.Key, g => g.ToList()),
                FormData = dto
            };

            return View(viewModel);
        }

        [Authorize(Roles = nameof(PossibleUserRole.JobSeeker))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditResumeDto formData)
        {
            if (!ModelState.IsValid)
            {
                var allSkills = await _getAllSkillsService.ExecuteAsync();
                var viewModel = new EditResumeViewModel
                {
                    Specialties = await _getAllSpecialtiesService.ExecuteAsync(),
                    GroupedSkills = allSkills.GroupBy(s => s.SkillType?.Name ?? "Інше").ToDictionary(g => g.Key, g => g.ToList()),
                    FormData = formData
                };
                return View(viewModel);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

            bool isSuccess = await _editResumeService.UpdateAsync(jobSeekerId, formData);

            if (!isSuccess)
            {
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

            TempData[ResumesConstants.SuccessMessageTempDataKey] = ResumesConstants.ResumeUpdatedSuccessMessage;
            return RedirectToAction(nameof(Details), new { id = formData.Id });
        }

        [Authorize(Roles = nameof(PossibleUserRole.JobSeeker))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int jobSeekerId)) return Unauthorized();

            bool isSuccess = await _deleteResumeService.DeleteAsync(id, jobSeekerId);

            if (!isSuccess)
            {
                TempData[ResumesConstants.ErrorMessageTempDataKey] = ResumesConstants.ResumeDeleteFailedMessage;
                return RedirectToAction(nameof(Details), new { id = id });
            }

            TempData[ResumesConstants.SuccessMessageTempDataKey] = ResumesConstants.ResumeDeletedSuccessMessage;

            return RedirectToAction("Index", "Profile");
        }
    }
}
