using Microsoft.AspNetCore.Mvc;

namespace CargoTetrisApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoadPlanController : ControllerBase
    {
        [HttpPost("execute")]
        public IActionResult Execute([FromBody] LoadPlanInput input)
        {
            var uld = input.Uld;
            var packages = input.Packages;

            // Simple greedy bin packing
            double cursorX = -uld.Width / 2;
            double cursorY = 0;
            double cursorZ = -uld.Length / 2;
            double maxRowHeight = 0;

            double uldRight = uld.Width / 2;
            double uldFront = uld.Length / 2;
            double uldHeight = uld.Height;

            var positions = new List<object>();

            foreach (var pkg in packages)
            {
                // If not enough space in X, move to next row (Z)
                if (cursorX + pkg.Width > uldRight)
                {
                    cursorX = -uld.Width / 2;
                    cursorZ += maxRowHeight;
                    maxRowHeight = 0;
                }
                // If not enough space in Z, move to next layer (Y)
                if (cursorZ + pkg.Length > uldFront)
                {
                    cursorZ = -uld.Length / 2;
                    cursorY += maxRowHeight;
                    maxRowHeight = 0;
                }
                // If not enough height, stop placing
                if (cursorY + pkg.Height > uldHeight)
                {
                    break; // Can't fit more packages
                }

                // Place the package
                positions.Add(new
                {
                    id = pkg.Id,
                    x = cursorX + pkg.Width / 2, // Center of the box
                    y = cursorY + pkg.Height / 2,
                    z = cursorZ + pkg.Length / 2,
                    width = pkg.Width,
                    length = pkg.Length,
                    height = pkg.Height
                });

                cursorX += pkg.Width;
                maxRowHeight = Math.Max(maxRowHeight, pkg.Length);
            }

            return Ok(new { status = "ok", positions });
        }
    }

    // ---- Models ----
    public class Uld
    {
        public string Code { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double TopWidth { get; set; }
    }

    public class Package
    {
        public string Id { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class LoadPlanInput
    {
        public Uld Uld { get; set; }
        public List<Package> Packages { get; set; }
    }
}
