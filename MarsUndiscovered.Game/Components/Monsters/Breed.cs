using System.Globalization;
using System.IO;
using CsvHelper;
using FrigidRogue.MonoGame.Core.Extensions;
using Microsoft.Xna.Framework;
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

        public static Dictionary<string, Breed> Breeds;
        public Attack MeleeAttack { get; protected set; } = null;
        public Attack LineAttack { get; protected set; } = null;
        public LightningAttack LightningAttack { get; protected set; } = null;
        public int DetectionRange { get; set; }
        
        public bool CanConcuss { get; set; }
        public bool WeaknessToConcuss { get; set; }
        public int SearchCooldown { get; set; }

        static Breed()
        {
            Breeds = new Dictionary<string, Breed>();

            using var reader = new StreamReader("Content\\Breeds.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            
            var csvBreeds = csv.GetRecords<dynamic>();
            foreach (var csvBreed in csvBreeds)
            {
                var breed = new Breed();
                
                breed.Name = csvBreed.Name.ToString();
                breed.NameWithoutSpaces = breed.Name.RemoveSpaces();
                breed.Description = csvBreed.Description.ToString();
                breed.DetectionRange = int.Parse(csvBreed.DetectionRange.ToString());
                breed.SearchCooldown = int.Parse(csvBreed.SearchCooldown);

                breed.CanConcuss = bool.Parse(csvBreed.CanConcuss);
                breed.WeaknessToConcuss = bool.Parse(csvBreed.WeaknessToConcuss);
                
                var bytes = int.Parse(csvBreed.CodePage437Character.ToString(), NumberStyles.HexNumber);
                
                breed.AsciiCharacter = (char)bytes;
                
                var foregroundColour = csvBreed.ForegroundColour.ToString();
                System.Drawing.Color sysDrawingForegroundColour = System.Drawing.ColorTranslator.FromHtml(foregroundColour);
                breed.ForegroundColour = new Color(sysDrawingForegroundColour.R, sysDrawingForegroundColour.G, sysDrawingForegroundColour.B, sysDrawingForegroundColour.A);
                
                if (!string.IsNullOrEmpty(csvBreed.BackgroundColour))
                {
                    var backgroundColour = csvBreed.BackgroundColour.ToString();
                    System.Drawing.Color sysDrawingBackgroundColour = System.Drawing.ColorTranslator.FromHtml(backgroundColour);
                    breed.BackgroundColour = new Color(sysDrawingBackgroundColour.R, sysDrawingBackgroundColour.G, sysDrawingBackgroundColour.B, sysDrawingBackgroundColour.A);
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

                if (!string.IsNullOrEmpty(csvBreed.IsWallTurret) && csvBreed.IsWallTurret.ToString().ToLower() == "true")
                {
                    breed.IsWallTurret = true;
                }
                
                Breeds.Add(breed.NameWithoutSpaces, breed);
            }
        }

        public static Breed GetBreed(string breed)
        {
            return Breeds[breed.RemoveSpaces()];
        }
    }
}