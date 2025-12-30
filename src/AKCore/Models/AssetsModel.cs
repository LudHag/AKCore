using System.Collections.Generic;

namespace AKCore.Models;

public record AssetModel(string Entrypoint, string[] Js, string[] Css);

public record AssetsModel(IDictionary<string, AssetModel> Assets);

public record EntryPointModel(AssetsModel AssetsModel, string EntryPointName);