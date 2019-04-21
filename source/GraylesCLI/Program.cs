using System;
using System.Linq;
using CommandLine;
using GraylesPlus;

namespace GraylesCLI
{
    class Program
    {

        [Verb("launch", HelpText = "Start starbound with the installed modpack.  Will not install the modpack if none are present.")]
        class LaunchOptions {
            
        }

        [Verb("install", HelpText = "Install a downloaded modpack.  For now, you must place Grayles Modpack V4.0.0.zip in the same directory as this program.")]
        class InstallOptions {
            [Option("pack", HelpText = "Which version of the modpack to install (ex: 4.0.0).")]
            public string Version { get; set; }

        }

        [Verb("config", HelpText = "Configure Grayles Plus for your system.")]
        class ConfigOptions {
            [Option("show")]
            public bool Show {get; set;}

            [Option("starbound", HelpText = "Configures the folder where Starbound is installed.")]
            public string StarboundRoot { get; set; }
        }


        static void Main(string[] args) => CommandLine.Parser.Default.ParseArguments<LaunchOptions, InstallOptions, ConfigOptions>(args)
                .MapResult(
                (LaunchOptions opts) => Launch(opts),
                (InstallOptions opts) => Install(opts),
                (ConfigOptions opts) => Configure(opts),
                errs => 1);

        private static int Launch(LaunchOptions options){
            System.Console.Out.WriteLine("Launching Starbound...");
            Config config = Config.Load();
            Starbound sb = new Starbound(config);
            if(!sb.Configured){
                Console.Out.WriteLine($"ERROR: Grayles is not yet installed!");
                return 1;
            }
            sb.Launch();
            return 0;
        }
        private static int Install(InstallOptions options){
            Config config = Config.Load();
            
            Mods mods = new Mods(config);
            OnlineUpdate update = OnlineUpdate.Fetch(config);
            if (!update.AvailableVersions.Any())
            {
                Console.Out.WriteLine("Sorry, I couldn't reach the update site to find versions.  Please try again later.");
                return 1;
            }

            if(options.Version != null) {
                ModpackVersion version = update.AvailableVersions.FirstOrDefault(v => v.VersionNumber == options.Version);
                if (version == null)
                {
                    Console.Out.WriteLine($"ERROR: Specified version not found.  Available versions: {string.Join(", ", update.AvailableVersions)}"); 
                    return 1;
                }
                mods = mods.With(targetVersion: version);
            } else {
                mods = mods.With(targetVersion: update.AvailableVersions.First(v=>v.Latest));
            }
            
            System.Console.Out.WriteLine($"Installing Grayles Plus pack {mods.TargetVersion}...");

            if(!mods.Downloaded){
                Console.Out.WriteLine($"ERROR: Please download {mods.ModZip} from the Grayles website!");
                return 1;
            }

            if(!mods.Install()){
                Console.Out.WriteLine("ERROR: Unable to unzip modpack!");
                return 1;
            };
            Console.Out.WriteLine("Modpack installed.  Configuring starbound..");

            Starbound sb = new Starbound(config);
            if(!sb.Configure()){
                Console.Out.WriteLine($"ERROR: Starbound at {sb.StarboundExecutableFolder} could not be configured!");
                return 1;
            }
            Console.Out.WriteLine($"Starbound at {sb.StarboundExecutableFolder} configured.");

            return 0;
        }
        private static int Configure(ConfigOptions options){
            Config config = Config.Load();
            if(options.StarboundRoot != null){
                config = config.With(starboundRoot: options.StarboundRoot);
                Config.Save(config);
            }
            if(options.Show){
                ShowConfig(config);
            }
            return 0;
        }

        private static void ShowConfig(Config config){
            Console.Out.WriteLine("Current configuration:\n====================");
            Console.Out.WriteLine($"Grayles installation folder: {config.GraylesRoot}");
            Console.Out.WriteLine($"Starbound folder: {config.StarboundRoot}");
        }
    }
}
