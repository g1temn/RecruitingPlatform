using RecruitingPlatform.DTOs.Common;
using RecruitingPlatform.Entities;

namespace RecruitingPlatform.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public string ActiveTab { get; set; } = "resumes";
    public PagedResultDto<Resume>? Resumes { get; set; }
    public PagedResultDto<Vacancy>? Vacancies { get; set; }
    public int CurrentPage { get; set; } = 1;
}