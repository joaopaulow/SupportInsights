namespace SupportInsights.Services
{ 
    public interface ITokenService
    {
        string GerarToken(string email, string role, string nome);
    }
}