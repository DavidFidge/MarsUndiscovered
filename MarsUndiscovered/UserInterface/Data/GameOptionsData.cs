using System.ComponentModel.DataAnnotations;

namespace MarsUndiscovered.UserInterface.Data
{
    public class GameOptionsData
    {
        [Display(Name = "Morgue File Username")]
        public string MorgueUsername { get; set; }

        [Display(Name = "Upload Morgue Files to Website")]
        public bool UploadMorgueFiles { get; set; }

        [Display(Name = "Use Ascii Tiles")]
        public bool UseAsciiTiles { get; set; }

        [Display(Name = "Enable Animations")]
        public bool UseAnimations { get; set; }
    }
}