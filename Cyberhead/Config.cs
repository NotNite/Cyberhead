using BepInEx.Configuration;

namespace Cyberhead;

public class Config(ConfigFile config) {
    public ConfigGeneral General = new(config);

    public class ConfigGeneral(ConfigFile config) {
        public ConfigEntry<bool> VrEnabled = config.Bind(
            "General",
            "VrEnabled",
            true,
            "Whether to set up VR mode, or just function as a compatibility addon for PC gameplay."
        );
    }
}
