namespace SupportInsights.Interfaces
{ 
    public interface ITokenService
    {
        string GerarToken(string email, string role, string nome);
    }
}