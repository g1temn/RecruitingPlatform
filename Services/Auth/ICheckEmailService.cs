namespace RecruitingPlatform.Services.Auth;

public interface ICheckEmailExsistsService
{
    Task<bool> ExecuteAcync(string email);
}
