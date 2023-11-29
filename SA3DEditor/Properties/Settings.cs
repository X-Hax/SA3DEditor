using SA3D.Rendering.Input;
using SA3D.Rendering.Input.Settings;
using System.Configuration;

namespace SA3DEditor.Properties
{
    internal class Settings : DebugInputSettings
    {
        public static Settings Default { get; } = (Settings)Synchronized(new Settings());

        [UserScopedSetting()]
        [InputCodeCategory("SA3D")]
        [InputCode("Focus Object", "Focuses camera to selected object when in orbit mode")]
        [DefaultSettingValue("F")]
        public InputCode FocusObj
        {
            get => (InputCode)this[nameof(FocusObj)];
            set => this[nameof(FocusObj)] = value;
        }

        [UserScopedSetting()]
        [InputCode("Swap Geometry", "Changes between rendering visual and collision geometry")]
        [DefaultSettingValue("F7")]
        public InputCode SwapGeometry
        {
            get => (InputCode)this[nameof(SwapGeometry)];
            set => this[nameof(SwapGeometry)] = value;
        }

        [UserScopedSetting()]
        [InputCode("Circle Object Relations", "Draws lines between object locations")]
        [DefaultSettingValue("F8")]
        public InputCode CircleObjectRelations
        {
            get => (InputCode)this[nameof(CircleObjectRelations)];
            set => this[nameof(CircleObjectRelations)] = value;
        }
    }
}
