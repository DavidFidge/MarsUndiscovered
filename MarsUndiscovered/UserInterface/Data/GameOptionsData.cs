using System.ComponentModel.DataAnnotations;

namespace MarsUndiscovered.UserInterface.Data
{
    public class GameOptionsData
    {
        [Display(Name = "Morgue File Username")]
        public string MorgueUsername { get; set; }
        
        [Display(Name = "Upload Morgue Files to Website")]
        public bool UploadMorgueFiles { get; set; }
    }
}