namespace AKCore.IntegrationTests.TestData;

public static class MenuEditForms
{
    public static FormUrlEncodedContent AddTopMenu(string name) =>
        new(new Dictionary<string, string> { ["name"] = name });

    public static FormUrlEncodedContent EditTopMenu(int menuId, string text) =>
        new(new Dictionary<string, string>
        {
            ["menuId"] = menuId.ToString(),
            ["text"] = text
        });

    public static FormUrlEncodedContent RemoveTopMenu(int menuId) =>
        new(new Dictionary<string, string> { ["id"] = menuId.ToString() });

    public static FormUrlEncodedContent RemoveSubMenu(int subMenuId) =>
        new(new Dictionary<string, string> { ["id"] = subMenuId.ToString() });

    public static FormUrlEncodedContent MoveLeft(int menuId) =>
        new(new Dictionary<string, string> { ["id"] = menuId.ToString() });

    public static FormUrlEncodedContent MoveRight(int menuId) =>
        new(new Dictionary<string, string> { ["id"] = menuId.ToString() });
}
