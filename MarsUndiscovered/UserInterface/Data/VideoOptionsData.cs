using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using DavidFidge.MonoGame.Core.Services;

namespace Augmented.UserInterface.Data
{
    public class VideoOptionsData
    {
        [Display(Name = "Screen Resolutions")]
        public IList<DisplayMode> DisplayModes { get; set; }
        public DisplayMode SelectedDisplayMode { get; set; }

        public bool IsFullScreen { get; set; }
        public bool IsVerticalSync { get; set; }

        public string Heading => "Video Options";
    }
}