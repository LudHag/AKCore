namespace AKCore.IntegrationTests.TestData;

public static class UserForms
{
    public static FormUrlEncodedContent CreateUser(
        string userName,
        string password = TestUsers.DefaultPassword,
        string firstName = "New",
        string lastName = "User") =>
        new(new Dictionary<string, string>
        {
            ["UserName"] = userName,
            ["Password"] = password,
            ["FirstName"] = firstName,
            ["LastName"] = lastName,
            ["Instrument"] = "Flöjt"
        });

    public static FormUrlEncodedContent EditUser(
        string id,
        string userName,
        string firstName = "Updated",
        string lastName = "User",
        string? medal = null,
        string? givenMedal = null) =>
        new(new Dictionary<string, string>
        {
            ["Id"] = id,
            ["UserName"] = userName,
            ["FirstName"] = firstName,
            ["LastName"] = lastName,
            ["Instrument"] = "Flöjt",
            ["Medal"] = medal ?? "",
            ["GivenMedal"] = givenMedal ?? ""
        });

    public static FormUrlEncodedContent AddRole(string userName, string role) =>
        new(new Dictionary<string, string>
        {
            ["UserName"] = userName,
            ["Role"] = role
        });

    public static FormUrlEncodedContent RemoveRole(string userName, string role) =>
        new(new Dictionary<string, string>
        {
            ["UserName"] = userName,
            ["Role"] = role
        });

    public static FormUrlEncodedContent ChangePassword(string userName, string password) =>
        new(new Dictionary<string, string>
        {
            ["userName"] = userName,
            ["password"] = password
        });
}
