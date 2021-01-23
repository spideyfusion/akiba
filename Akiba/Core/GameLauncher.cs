namespace Akiba.Core
{
    using Akiba.Core.Exceptions;
    using System;
    using System.Diagnostics;
    using System.IO;

    internal class GameLauncher
    {
        private enum GameVariant : ushort
        {
            Gog,
            Steam,
        };

        private const uint SteamApplicationId = 333980;

        private const string GogIdentifierFile = "goggame-2053169534.id";
        private const string SteamIdentifierFile = "steam_appid.txt";

        public static void Launch()
        {
            switch (DetectGameVariant())
            {
                case GameVariant.Gog:
                    _ = Process.Start(Utilities.GameExecutableName);
                    return;
                case GameVariant.Steam:
                    _ = Process.Start(string.Format("steam://run/{0}", SteamApplicationId));
                    return;
                default:
                    break;
            }
        }

        private static GameVariant DetectGameVariant()
        {
            if (File.Exists(GogIdentifierFile))
            {
                return GameVariant.Gog;
            }

            var steamApplicationId = File.Exists(SteamIdentifierFile) ? Convert.ToInt32(File.ReadAllText(SteamIdentifierFile)) : 0;

            if (steamApplicationId == SteamApplicationId)
            {
                return GameVariant.Steam;
            }

            throw new UnsupportedGameVariantException();
        }
    }
}
