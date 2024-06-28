using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using VipCoreApi;
using LevelsRanksApi;
namespace VIP_ExperienceMultiplier;

public class VipExperienceMultiplier : BasePlugin
{
    public override string ModuleAuthor => "ABKAM";
    public override string ModuleName => "[VIP] Experience Multiplier";
    public override string ModuleVersion => "v1.0.0";

    private IVipCoreApi? _api;
    private ExperienceMultiplier? _experienceMultiplier;

    private PluginCapability<IVipCoreApi> PluginCapability { get; } = new("vipcore:core");
    private PluginCapability<ILevelsRanksApi> LevelsRanksCapability { get; } = new("levels_ranks");

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _api = PluginCapability.Get();
        var levelsRanksApi = LevelsRanksCapability.Get();

        if (_api == null || levelsRanksApi == null) return;

        _experienceMultiplier = new ExperienceMultiplier(_api, levelsRanksApi);
        _api.RegisterFeature(_experienceMultiplier);
    }

    public override void Unload(bool hotReload)
    {
        _api?.UnRegisterFeature(_experienceMultiplier!);
    }
}

public class ExperienceMultiplier : VipFeatureBase
{
    private readonly ILevelsRanksApi _levelsRanksApi;

    public override string Feature => "ExperienceMultiplier";

    public ExperienceMultiplier(IVipCoreApi api, ILevelsRanksApi levelsRanksApi) : base(api)
    {
        _levelsRanksApi = levelsRanksApi;
    }

    public override void OnPlayerSpawn(CCSPlayerController player)
    {
        if (!PlayerHasFeature(player)) return;
        if (GetPlayerFeatureState(player) is not IVipCoreApi.FeatureState.Enabled) return;

        var steamId = player.SteamID.ToString();
        var multiplier = GetFeatureValue<double>(player);

        if (multiplier <= 0) return;

        _levelsRanksApi.SetExperienceMultiplier(steamId, multiplier);
    }
}
