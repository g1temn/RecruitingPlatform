namespace RecruitingPlatform.Const.Vacancies;

public static class VacanciesConstants
{
    public const int NumberOfVacanciesOnOnePage = 25;
    public const string SuccessMessageTempDataKey = "SuccessMessage";
    public const string ErrorMessageTempDataKey = "ErrorMessage";
    public const string VacancyUpdatedSuccessMessage = "Вакансію успішно оновлено!";
    public const string VacancyUpdateFailedMessage = "Не вдалося оновити вакансію. Можливо, вона була видалена або ви не маєте до неї доступу.";
    public const string VacancyDeletedSuccessMessage = "Вакансію успішно видалено!";
    public const string VacancyDeleteFailedMessage = "Не вдалося видалити вакансію.";
    public const string VacancyNotFoundErrorMessage = "Вакансію не знайдено.";
    public const string TitleRequired = "Назва вакансії є обов'язковою.";
    public const string TitleMaxLength = "Назва вакансії не може перевищувати 150 символів.";
    public const string SpecialtyRequired = "Будь ласка, оберіть спеціальність.";
    public const string DescriptionRequired = "Опис вакансії є обов'язковим.";
    public const string MinSalaryRange = "Мінімальна зарплата повинна бути в межах від 0 до 1 000 000.";
    public const string MaxSalaryRange = "Максимальна зарплата повинна бути в межах від 0 до 1 000 000.";
    public const string CurrencyRequired = "Будь ласка, оберіть валюту.";
    public const string InvalidSalaryRange = "Мінімальна зарплата не може бути більшою за максимальну.";
}