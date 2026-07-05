using AKCore.DataModel;

namespace AKCore.IntegrationTests.TestData;

public static class TestMenus
{
    public const string TopMenuName = "Test Top Menu";
    public const string EditedTopMenuName = "Edited Top Menu";
    public const string SubMenuName = "Test Sub Menu";

    public static Menu TopMenu(string name = TopMenuName, int posIndex = 0) =>
        new()
        {
            Name = name,
            PosIndex = posIndex
        };

    public static SubMenu SubMenu(string name = SubMenuName, int subPosIndex = 0) =>
        new()
        {
            Name = name,
            SubPosIndex = subPosIndex
        };
}
