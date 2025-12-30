using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AKCore.Models;

public record AssetModel(string Entrypoint, string[] Js, string[] Css);

public record AssetsModel(IDictionary<string, AssetModel> Assets)
{
    public AssetModel GetAssetForEntrypoint(string entrypoint, ViewDataDictionary viewData)
    {
        if (!Assets.TryGetValue(entrypoint, out var asset))
            return new AssetModel("", [], []);

        var registeredJs = GetRegisteredSet(viewData, "RegisteredJs");
        var registeredCss = GetRegisteredSet(viewData, "RegisteredCss");

        var newJs = asset.Js.Where(registeredJs.Add).ToArray();
        var newCss = asset.Css.Where(registeredCss.Add).ToArray();

        return new AssetModel(asset.Entrypoint, newJs, newCss);
    }

    private HashSet<string> GetRegisteredSet(ViewDataDictionary viewData, string key)
    {
        if (viewData[key] is not HashSet<string> set)
            viewData[key] = set = [];
        return set;
    }
}

public record EntryPointModel(AssetsModel AssetsModel, string EntryPointName);