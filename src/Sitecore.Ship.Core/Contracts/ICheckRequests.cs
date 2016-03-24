namespace Sitecore.Ship.Core.Contracts
{
    public interface ICheckRequests
    {
        bool IsLocal { get; }

        string UserHostAddress { get; }

        string AuthToken(string configAuthHeader);
    }
}