namespace DarkKitchen.Domain.Validators;

public sealed class UruguayPhoneValidator : IPhoneValidator
{
    public bool IsValid(string phone)
    {
        var cleaned = phone.Replace(" ", string.Empty);

        if(phone.Any(char.IsLetter))
        {
            return false;
        }

        if(!System.Text.RegularExpressions.Regex.IsMatch(cleaned, @"^09\d{7}$"))
        {
            return false;
        }

        return true;
    }
}
