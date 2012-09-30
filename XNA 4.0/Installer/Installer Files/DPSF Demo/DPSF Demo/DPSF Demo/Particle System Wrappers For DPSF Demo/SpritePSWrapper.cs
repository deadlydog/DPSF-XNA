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
	class SpriteDPSFDemoParticleSystemWrapper : SpriteParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public SpriteDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            if (this.Name.Equals("Sprite Force") || this.Name.Equals("Sprite Cloud"))
            {
                draw.TextWriter.DrawString(draw.Font, "Force:", new Vector2(draw.TextSafeArea.Left + 200, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
                draw.TextWriter.DrawString(draw.Font, this.AttractorMode.ToString(), new Vector2(draw.TextSafeArea.Left + 260, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);

                draw.TextWriter.DrawString(draw.Font, "Strength:", new Vector2(draw.TextSafeArea.Left + 410, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
                draw.TextWriter.DrawString(draw.Font, this.AttractorStrength.ToString("0.0"), new Vector2(draw.TextSafeArea.Left + 495, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);

				draw.TextWriter.DrawString(draw.Font, "Use the mouse", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 225), draw.ControlTextColor);
            }
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Mouse Attraction:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(167, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Mouse Cloud:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(125, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Grid:", new Vector2(5, 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(50, 300), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Rotators:", new Vector2(5, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(87, 325), draw.ControlTextColor);

            if (this.Name.Equals("Sprite Force") || this.Name.Equals("Sprite Cloud"))
            {
                draw.TextWriter.DrawString(draw.Font, "Toggle Force:", new Vector2(5, 350), draw.PropertyTextColor);
				draw.TextWriter.DrawString(draw.Font, "Left Mouse Button", new Vector2(130, 350), draw.ControlTextColor);

                draw.TextWriter.DrawString(draw.Font, "Toggle Strength:", new Vector2(5, 375), draw.PropertyTextColor);
				draw.TextWriter.DrawString(draw.Font, "Right Mouse Button", new Vector2(155, 375), draw.ControlTextColor);
            }
	    }

	    public void ProcessInput()
	    {
            // If the mouse was moved
            if (MouseManager.CurrentMouseState.X != MouseManager.PreviousMouseState.X ||
                MouseManager.CurrentMouseState.Y != MouseManager.PreviousMouseState.Y)
            {
                this.AttractorPosition = new Vector3(MouseManager.CurrentMouseState.X, MouseManager.CurrentMouseState.Y, 0);
            }

            // If the left mouse button was just pressed
            if (MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                MouseManager.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                this.ToggleAttractorMode();
            }

            // If the right mouse button was just pressed
            if (MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed &&
                MouseManager.PreviousMouseState.RightButton == ButtonState.Released)
            {
                this.ToggleAttractorStrength();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadAttractionEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadCloudEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.V))
            {
                this.LoadGridEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.B))
            {
                this.LoadRotatorsEvents();
            }
	    }
	}
}