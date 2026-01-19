using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using CsvHelper;
using FrigidRogue.MonoGame.Core.Extensions;
using MonoGame.Extended;

namespace MarsUndiscovered.Game.Components
{
    public class Breed : GameObjectType
    {
        public string NameWithoutSpaces { get; set;}
        public string Description { get; set; }
        public int MaxHealth { get; set; }
        public decimal RegenRate { get; set; }
        public bool IsWallTurret { get; set; }
        public bool FriendlyFireAllies { get; set; } = true;

        private static Dictionary<string, Breed> _breeds;
        public static Dictionary<string, Breed> Breeds
        {
            get
            {
                if (_breeds == null)
                    _breeds = GetBreeds();

                return _breeds;
            }
            set
            {
                _breeds = value;
            }
        }

        public Attack MeleeAttack { get; protected set; }
        public Attack LineAttack { get; protected set; }
        public LightningAttack LightningAttack { get; protected set; }
        public int DetectionRange { get; set; }
        
        public bool CanConcuss { get; set; }
        public bool WeaknessToConcuss { get; set; }
        public int SearchCooldown { get; set; }
        public int TimeToMove { get; set; }


        public static Dictionary<string, Breed> GetBreeds()
        {
            var breeds = new Dictionary<string, Breed>();

            using var reader = new StreamReader("Content\\Breeds.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var csvBreeds = csv.GetRecords<dynamic>();
            foreach (var csvBreed in csvBreeds)
            {
                // Skip empty lines
                if (string.IsNullOrEmpty(csvBreed.Name))
                    continue;

                var breed = new Breed();

                breed.Name = csvBreed.Name.ToString();
                breed.NameWithoutSpaces = breed.Name.RemoveSpaces();
                breed.Description = csvBreed.Description.ToString();
                breed.DetectionRange = int.Parse(csvBreed.DetectionRange.ToString());
                breed.SearchCooldown = int.Parse(csvBreed.SearchCooldown);
                breed.TimeToMove = ((string)csvBreed.TimeToMove).ParseIntOrDefault(1);

                breed.CanConcuss = ((string)csvBreed.CanConcuss).ParseBoolOrFalse();
                breed.WeaknessToConcuss = ((string)csvBreed.WeaknessToConcuss).ParseBoolOrFalse();

                var bytes = int.Parse(csvBreed.CodePage437Character.ToString(), NumberStyles.HexNumber);

                breed.AsciiCharacter = (char)bytes;

                var foregroundColour = csvBreed.ForegroundColour.ToString();
                Color sysDrawingForegroundColour = ColorTranslator.FromHtml(foregroundColour);
                breed.ForegroundColour = new Microsoft.Xna.Framework.Color(sysDrawingForegroundColour.R, sysDrawingForegroundColour.G, sysDrawingForegroundColour.B, sysDrawingForegroundColour.A);

                if (!string.IsNullOrEmpty(csvBreed.BackgroundColour))
                {
                    var backgroundColour = csvBreed.BackgroundColour.ToString();
                    Color sysDrawingBackgroundColour = ColorTranslator.FromHtml(backgroundColour);
                    breed.BackgroundColour = new Microsoft.Xna.Framework.Color(sysDrawingBackgroundColour.R, sysDrawingBackgroundColour.G, sysDrawingBackgroundColour.B, sysDrawingBackgroundColour.A);
                }

                breed.MaxHealth = int.Parse(csvBreed.MaxHealth.ToString());

                if (!string.IsNullOrEmpty(csvBreed.RegenRate))
                    breed.RegenRate = decimal.Parse(csvBreed.RegenRate.ToString());

                if (!string.IsNullOrEmpty(csvBreed.BasicAttackMin))
                {
                    var damageRange = new Range<int>(int.Parse(csvBreed.BasicAttackMin.ToString()), int.Parse(csvBreed.BasicAttackMax.ToString()));
                    breed.MeleeAttack = new Attack(damageRange);
                }

                if (!string.IsNullOrEmpty(csvBreed.LineAttackMin))
                {
                    var damageRange = new Range<int>(int.Parse(csvBreed.LineAttackMin.ToString()), int.Parse(csvBreed.LineAttackMax.ToString()));
                    breed.LineAttack = new Attack(damageRange);
                }

                if (!string.IsNullOrEmpty(csvBreed.LightningAttack))
                {
                    breed.LightningAttack = new LightningAttack(int.Parse(csvBreed.LightningAttack.ToString()));
                }

                breed.IsWallTurret = ((string)csvBreed.IsWallTurret).ParseBoolOrFalse();

                breeds.Add(breed.NameWithoutSpaces, breed);
            }

            return breeds;
        }

        public static Breed GetBreed(string breed)
        {
            // Used to have this in a static constructor but you don't get a good stack
            // trace in that
            if (Breeds == null)
                Breeds = GetBreeds();

            return Breeds[breed.RemoveSpaces()];
        }
    }
}