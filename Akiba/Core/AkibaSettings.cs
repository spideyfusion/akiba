using System;
using System.IO;

namespace Akiba.Core
{
    class AkibaSettings : IDisposable
    {
        private FileStream ConfigurationStream;

        private enum ConfigurationPositions : int
        {
            RenderingResolutionWidth = 8,
            RenderingResolutionHeight = 12,
            Fullscreen = 16,
            VerticalSynchronization = 684,
            AntiAliasing = 688,
        }

        public AkibaSettings()
        {
            string configurationPath = string.Format(
                @"{0}\AKIBA'S TRIP\config.dat",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            );

            if (!File.Exists(configurationPath))
            {
                File.WriteAllBytes(configurationPath, Properties.Resources.DefaultGameConfig);
            }

            this.ConfigurationStream = new FileStream(configurationPath, FileMode.Open, FileAccess.ReadWrite);
        }

        public void Dispose()
        {
            this.ConfigurationStream.Close();
        }

        public void ApplySettings()
        {
            this.SetConfigurationValue(ConfigurationPositions.RenderingResolutionWidth, Program.Config.RenderingResolutionWidth);
            this.SetConfigurationValue(ConfigurationPositions.RenderingResolutionWidth + 1, (Program.Config.RenderingResolutionWidth >> 8));

            this.SetConfigurationValue(ConfigurationPositions.RenderingResolutionHeight, Program.Config.RenderingResolutionHeight);
            this.SetConfigurationValue(ConfigurationPositions.RenderingResolutionHeight + 1, (Program.Config.RenderingResolutionHeight >> 8));

            this.SetConfigurationValue(ConfigurationPositions.Fullscreen, Convert.ToInt16(Program.Config.Fullscreen));
            this.SetConfigurationValue(ConfigurationPositions.VerticalSynchronization, Convert.ToInt16(Program.Config.VerticalSynchronization));
            this.SetConfigurationValue(ConfigurationPositions.AntiAliasing, Convert.ToInt16(Program.Config.AntiAliasing));
        }

        private void SetConfigurationValue(ConfigurationPositions position, int value)
        {
            this.ConfigurationStream.Position = (int)position;

            this.ConfigurationStream.WriteByte((byte)value);
        }
    }
}
