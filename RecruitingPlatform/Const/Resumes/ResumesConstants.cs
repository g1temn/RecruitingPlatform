namespace RecruitingPlatform.Const.Resumes;

public static class ResumesConstants
{
    public const int NumberOfResumesOnOnePage = 20;
    public const string SuccessMessageTempDataKey = "SuccessMessage";
    public const string ErrorMessageTempDataKey = "ErrorMessage";
    public const string ResumeCreatedSuccessMessage = "Ваше резюме успішно створено!";
    public const string ResumeCreationErrorMessage = "Виникла помилка при збереженні резюме.";
    public const string ResumeUpdatedSuccessMessage = "Резюме успішно оновлено!";
    public const string ResumeUpdateFailedMessage = "Не вдалося оновити резюме. Можливо, воно було видалене або ви не маєте до нього доступу.";
    public const string ResumeDeletedSuccessMessage = "Резюме успішно видалено!";
    public const string ResumeDeleteFailedMessage = "Не вдалося видалити резюме.";
    public const string ResumeNotFoundErrorMessage = "Резюме не знайдено.";
    public const string TitleRequired = "Посада (заголовок) є обов'язковою.";
    public const string TitleMaxLength = "Заголовок не може перевищувати 150 символів.";
    public const string SpecialtyRequired = "Будь ласка, оберіть спеціальність.";
    public const string SummaryRequired = "Опис (Про себе) є обов'язковим згідно з вимогами.";
}