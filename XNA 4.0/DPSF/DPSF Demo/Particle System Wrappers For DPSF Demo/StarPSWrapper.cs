using BasicVirtualEnvironment.Input;
using DPSF_Demo.ParticleSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class StarDPSFDemoParticleSystemWrapper : StarParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public StarDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Emitter Intermittence Mode:", new Vector2(draw.TextSafeArea.Left + 180, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, this.miIntermittanceTimeMode.ToString("0"), new Vector2(draw.TextSafeArea.Left + 435, draw.TextSafeArea.Top + 2), draw.PropertyTextColor);
	    }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "2D Star:", new Vector2(5, 250), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "X", new Vector2(85, 250), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "3D Star:", new Vector2(5, 275), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "C", new Vector2(85, 275), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Hold and Rotate Emitter:", new Vector2(5, 300), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Adjust Rotational Velocity:", new Vector2(15, 325), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "V", new Vector2(260, 325), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Adjust Rotational Acceleration:", new Vector2(15, 350), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "B", new Vector2(300, 350), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Wiggle Mode:", new Vector2(15, 375), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "P", new Vector2(140, 375), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Reset Rotational Forces:", new Vector2(5, 400), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "N", new Vector2(230, 400), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Highlight Axis:", new Vector2(5, 425), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "M", new Vector2(135, 425), draw.ControlTextColor);

            draw.TextWriter.DrawString(draw.Font, "Toggle Emitter Intermittence:", new Vector2(5, 450), draw.PropertyTextColor);
			draw.TextWriter.DrawString(draw.Font, "[", new Vector2(270, 450), draw.ControlTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyWasJustPressed(Keys.X))
            {
                this.ParticleInitializationFunction = this.InitializeParticleStar2D;
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.C))
            {
                this.ParticleInitializationFunction = this.InitializeParticleStar3D;
            }

            float fRotationScale = MathHelper.Pi / 18.0f;

            if (KeyboardManager.KeyIsDown(Keys.V))
            {
                // Check if the Emitter is being rotated
                if (KeyboardManager.KeyWasJustPressed(Keys.J))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Down * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.L))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Up * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.I))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Left * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.K))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Right * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.U))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Backward * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.O))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Forward * fRotationScale;
                }
            }

            if (KeyboardManager.KeyIsDown(Keys.B))
            {
                // Check if the Emitter is being rotated
                if (KeyboardManager.KeyWasJustPressed(Keys.J))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Down * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.L))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Up * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.I))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Left * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.K))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Right * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.U))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Backward * fRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.O))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Forward * fRotationScale;
                }
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.Emitter.PositionData.Velocity = Vector3.Zero;
                this.Emitter.PositionData.Acceleration = Vector3.Zero;
                this.Emitter.OrientationData.RotationalVelocity = Vector3.Zero;
                this.Emitter.OrientationData.RotationalAcceleration = Vector3.Zero;
                this.ParticleSystemEvents.RemoveAllEvents();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.M))
            {
                this.mbHighlightAxis = !this.mbHighlightAxis;
            }

            if (KeyboardManager.KeyIsDown(Keys.P))
            {
                // Check if the Emitter is being rotated
                if (KeyboardManager.KeyWasJustPressed(Keys.J))
                {
                    this.LoadSlowRotationalWiggle();
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.L))
                {
                    this.LoadFastRotationalWiggle();
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.I))
                {
                    this.LoadMediumWiggle();
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.K))
                {
                    this.LoadMediumRotationalWiggle();
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.U))
                {
                    this.LoadSlowWiggle();
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.O))
                {
                    this.LoadFastWiggle();
                }
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.OemOpenBrackets))
            {
                this.ToggleEmitterIntermittance();
            }
	    }
	}
}