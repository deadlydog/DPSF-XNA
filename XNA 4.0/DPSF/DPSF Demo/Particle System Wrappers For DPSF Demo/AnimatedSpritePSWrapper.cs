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
	class AnimatedSpriteDPSFDemoParticleSystemWrapper : AnimatedSpriteParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public AnimatedSpriteDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
			draw.TextWriter.DrawString(draw.Font, "Use the mouse", new Vector2(draw.TextSafeArea.Left + 5, draw.TextSafeArea.Top + 225), draw.ControlTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Explosion:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(95, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Butterfly:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(90, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Color Mode:", new Vector2(5, 300), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "Left Mouse Button", new Vector2(183, 300), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Add Particle:", new Vector2(5, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "Right Mouse Button", new Vector2(125, 325), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            // If the mouse was moved
            if (MouseManager.CurrentMouseState.X != MouseManager.PreviousMouseState.X ||
                MouseManager.CurrentMouseState.Y != MouseManager.PreviousMouseState.Y)
            {
                this.MousePosition = new Vector3(MouseManager.CurrentMouseState.X, MouseManager.CurrentMouseState.Y, 0);
            }

            // If the left mouse button was just pressed
            if (MouseManager.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                MouseManager.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                this.ToggleColorAmount();
            }

            // If the right mouse button was just pressed
            if (MouseManager.CurrentMouseState.RightButton == ButtonState.Pressed &&
                MouseManager.PreviousMouseState.RightButton == ButtonState.Released)
            {
                this.AddParticle();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.LoadExplosionEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.LoadButterflyEvents();
            }
	    }
	}
}