namespace Akiba.Core
{
    using System;
    using System.IO;

    internal class AkibaSettings : IDisposable
    {
        private readonly FileStream configurationStream;
        private readonly IniManipulator iniManipulator;

        private enum ConfigurationPositions : uint
        {
            RenderingResolutionWidth = 8,
            RenderingResolutionHeight = 12,
            Fullscreen = 16,
            VerticalSynchronization = 684,
            AntiAliasing = 688,
            DisableMovies = 700,
        }

        public AkibaSettings()
        {
            var configurationPath = string.Format(
                @"{0}\AKIBA'S TRIP\config.dat",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            );

            if (!File.Exists(configurationPath))
            {
                File.WriteAllBytes(configurationPath, Properties.Resources.DefaultGameConfig);
            }

            this.configurationStream = new FileStream(configurationPath, FileMode.Open, FileAccess.ReadWrite);
            this.iniManipulator = new IniManipulator(Utilities.GameIniName);
        }

        public void ApplySettings()
        {
            this.SetConfigurationValue(ConfigurationPositions.RenderingResolutionWidth, Program.Config.RenderingResolutionWidth);
            this.SetConfigurationValue(ConfigurationPositions.RenderingResolutionWidth + 1, Program.Config.RenderingResolutionWidth >> 8);

            this.SetConfigurationValue(ConfigurationPositions.RenderingResolutionHeight, Program.Config.RenderingResolutionHeight);
            this.SetConfigurationValue(ConfigurationPositions.RenderingResolutionHeight + 1, Program.Config.RenderingResolutionHeight >> 8);

            this.SetConfigurationValue(ConfigurationPositions.Fullscreen, Convert.ToInt16(Program.Config.ScreenMode == Configuration.ScreenModes.Fullscreen));
            this.SetConfigurationValue(ConfigurationPositions.VerticalSynchronization, Convert.ToInt16(Program.Config.VerticalSynchronization));
            this.SetConfigurationValue(ConfigurationPositions.AntiAliasing, Convert.ToInt16(Program.Config.AntiAliasing));
            this.SetConfigurationValue(ConfigurationPositions.DisableMovies, Convert.ToInt16(Program.Config.DisableMovies));

            this.iniManipulator.Write("GameFPS", Program.Config.FramesPerSecond.ToString(), "Display");
        }

        public void Dispose() => this.configurationStream.Close();

        private void SetConfigurationValue(ConfigurationPositions position, int value)
        {
            this.configurationStream.Position = (uint)position;

            this.configurationStream.WriteByte((byte)value);
        }
    }
}
