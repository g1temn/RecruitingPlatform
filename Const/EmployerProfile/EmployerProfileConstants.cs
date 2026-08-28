namespace RecruitingPlatform.Const.Employers;

public static class EmployerProfileConstants
{
    public const string SuccessMessageTempDataKey = "SuccessMessage";
    public const string ProfileUpdatedSuccessMessage = "Профіль компанії успішно оновлено!";
    public const string ProfileNotFoundErrorMessage = "Профіль компанії не знайдено.";
    public const string ProfileUpdateFailedMessage = "Не вдалося оновити профіль. Компанію не знайдено або виникла помилка бази даних.";
    public const string NameRequired = "Назва компанії є обов'язковою.";
    public const string NameMaxLength = "Назва компанії не може перевищувати 100 символів.";
    public const string PhoneMaxLength = "Телефон не може перевищувати 20 символів.";
    public const string WebsiteInvalidFormat = "Невірний формат посилання (URL).";
    public const string WebsiteMaxLength = "Посилання не може перевищувати 255 символів.";
}