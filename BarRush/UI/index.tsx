import { ModRegistrar } from "cs2/modding";
import { DjPopup } from "mods/DjPopup";
import { BarDashboard } from "mods/BarDashboard";

const register: ModRegistrar = (moduleRegistry) => {
    moduleRegistry.append("GameTopRight", BarDashboard);
    moduleRegistry.append("GameTopRight", DjPopup);
};

export default register;
