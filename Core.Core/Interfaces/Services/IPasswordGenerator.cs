namespace Api.Core.Interfaces.Services
{
    public interface IPasswordGenerator
    {
        string Generate(
            int requiredLengthMin = 60,
            int requiredLengthMax = 68,
            int requiredUniqueChars = 8,
            bool requireDigit = true,
            bool requireLowercase = true,
            bool requireNonAlphanumeric = true,
            bool requireUppercase = true);
        
        string GenerateNoSpecial(
            int requiredLengthMin = 60,
            int requiredLengthMax = 68,
            int requiredUniqueChars = 8,
            bool requireDigit = true,
            bool requireLowercase = true,
            bool requireUppercase = true);
    }
}