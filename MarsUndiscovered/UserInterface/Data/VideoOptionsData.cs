using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using FrigidRogue.MonoGame.Core.Services;

namespace MarsUndiscovered.UserInterface.Data
{
    public class VideoOptionsData
    {
        [Display(Name = "Screen Resolution")]
        public IList<DisplayDimension> DisplayDimensions { get; set; }
        public DisplayDimension SelectedDisplayDimension { get; set; }

        [Display(Name = "Render Size")]
        public IList<RenderResolution> RenderResolutions { get; set; }
        public RenderResolution SelectedRenderResolution { get; set; }
        public bool IsFullScreen { get; set; }
        public bool IsBorderlessWindowed { get; set; }
        public bool IsVerticalSync { get; set; }
        public string Heading => "Video Options";
    }
}