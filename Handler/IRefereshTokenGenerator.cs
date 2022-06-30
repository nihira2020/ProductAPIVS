namespace ProductAPIVS.Handler;
public interface IRefereshTokenGenerator{

    Task<string> GenerateToken(string username);

}