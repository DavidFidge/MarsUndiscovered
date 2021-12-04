using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using FrigidRogue.MonoGame.Core.Services;

namespace MarsUndiscovered.UserInterface.Data
{
    public class VideoOptionsData
    {
        [Display(Name = "Screen Resolutions")]
        public IList<DisplayDimensions> DisplayModes { get; set; }
        public DisplayDimensions SelectedDisplayDimensions { get; set; }

        public bool IsFullScreen { get; set; }
        public bool IsBorderlessWindowed { get; set; }
        public bool IsVerticalSync { get; set; }

        public string Heading => "Video Options";
    }
}