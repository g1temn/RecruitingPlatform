namespace RecruitingPlatform.Const.JobSeekers;

public static class JobSeekerProfileConstants
{
    public const string SuccessMessageTempDataKey = "SuccessMessage";
    public const string ProfileUpdatedSuccessMessage = "Ваш профіль успішно оновлено!";
    public const string ProfileNotFoundErrorMessage = "Профіль не знайдено.";
    public const string ProfileUpdateFailedMessage = "Не вдалося оновити профіль. Користувача не знайдено або виникла помилка бази даних.";
    public const string FirstNameRequired = "Ім'я є обов'язковим.";
    public const string FirstNameMaxLength = "Ім'я не може перевищувати 100 символів.";
    public const string LastNameRequired = "Прізвище є обов'язковим.";
    public const string LastNameMaxLength = "Прізвище не може перевищувати 100 символів.";
    public const string PhoneRequired = "Контактний телефон є обов'язковим.";
    public const string PhoneMaxLength = "Телефон не може перевищувати 20 символів.";
    public const string EmailRequired = "Контактний email є обов'язковим.";
    public const string EmailInvalidFormat = "Невірний формат email.";
    public const string EmailMaxLength = "Email не може перевищувати 255 символів.";
    public const string BirthdayRequired = "Дата народження є обов'язковою.";
}