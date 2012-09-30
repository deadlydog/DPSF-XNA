using System;
using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
#if (WINDOWS)
	[Serializable]
#endif
	class ImageDPSFDemoParticleSystemWrapper : ImageParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public ImageDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Rows:", new Vector2(draw.TextSafeArea.Left + 260, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.miNumberOfRows.ToString("0"), new Vector2(draw.TextSafeArea.Left + 320, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Columns:", new Vector2(draw.TextSafeArea.Left + 350, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.miNumberOfColumns.ToString("0"), new Vector2(draw.TextSafeArea.Left + 435, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Spin Mode:", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 450), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.msSpinMode, new Vector2(draw.TextSafeArea.Left + 105, draw.TextSafeArea.Top + 450), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Uniform:", new Vector2(draw.TextSafeArea.Left + 170, draw.TextSafeArea.Top + 450), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.mbUniformSpin.ToString(), new Vector2(draw.TextSafeArea.Left + 245, draw.TextSafeArea.Top + 450), draw.PropertyTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Image:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(65, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Spiral:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(60, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Vortex:", new Vector2(5, 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(75, 300), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Spin Mode:", new Vector2(5, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(175, 325), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Uniform Spin:", new Vector2(5, 350), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "N", new Vector2(195, 350), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Scatter Image:", new Vector2(5, 375), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "M", new Vector2(140, 375), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Rows:", new Vector2(5, 400), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "P", new Vector2(130, 400), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Columns:", new Vector2(5, 425), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "[", new Vector2(155, 425), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadImage();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadSpiralIntoFinalImage();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.LoadVortexIntoFinalImage();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                string sSpinMode = this.msSpinMode;

                switch (sSpinMode)
                {
                    case "Pitch":
                        sSpinMode = "Yaw";
                        break;

                    case "Yaw":
                        sSpinMode = "Roll";
                        break;

                    case "Roll":
                        sSpinMode = "All";
                        break;

                    case "All":
                        sSpinMode = "None";
                        break;

                    default:
                    case "None":
                        sSpinMode = "Pitch";
                        break;
                }

                this.ToggleSpin(sSpinMode);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.ToggleUniformSpin();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.M))
            {
                this.Scatter();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.P))
            {
                int iRows = this.miNumberOfRows;
                int iColumns = this.miNumberOfColumns;

                switch (iRows)
                {
                    default:
                    case 2:
                        iRows = 4;
                        break;

                    case 4:
                        iRows = 8;
                        break;

                    case 8:
                        iRows = 16;
                        break;

                    case 16:
                        iRows = 32;
                        break;

                    case 32:
                        iRows = 64;
                        break;

                    case 64:
                        iRows = 2;
                        break;
                }

                this.SetNumberOfRowsAndColumns(iRows, iColumns);
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.OemOpenBrackets))
            {
                int iRows = this.miNumberOfRows;
                int iColumns = this.miNumberOfColumns;

                switch (iColumns)
                {
                    default:
                    case 2:
                        iColumns = 4;
                        break;

                    case 4:
                        iColumns = 8;
                        break;

                    case 8:
                        iColumns = 16;
                        break;

                    case 16:
                        iColumns = 32;
                        break;

                    case 32:
                        iColumns = 64;
                        break;

                    case 64:
                        iColumns = 2;
                        break;
                }

                this.SetNumberOfRowsAndColumns(iRows, iColumns);
            }
	    }
	}
}