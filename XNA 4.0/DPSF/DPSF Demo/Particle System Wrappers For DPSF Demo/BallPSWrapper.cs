using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPSF.ParticleSystems;
using DPSF_Demo.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DPSF_Demo.Particle_System_Wrappers_For_DPSF_Demo
{
	class BallDPSFDemoParticleSystemWrapper : BallParticleSystem, IWrapDPSFDemoParticleSystems
	{
        public BallDPSFDemoParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void DrawStatusText(DrawTextRequirements draw)
	    { }

	    public void DrawInputControlsText(DrawTextRequirements draw)
	    {
            draw.TextWriter.DrawString(draw.Font, "Increase Radius:", new Vector2(5, 250), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "X", new Vector2(155, 250), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Decrease Radius:", new Vector2(5, 275), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "C", new Vector2(165, 275), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Hold and Rotate Emitter:", new Vector2(5, 300), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Adjust Rotational Velocity:", new Vector2(15, 325), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "V", new Vector2(260, 325), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Adjust Rotational Acceleration:", new Vector2(15, 350), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "B", new Vector2(300, 350), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Reset Rotational Forces:", new Vector2(5, 375), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "N", new Vector2(230, 375), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Less Particles:", new Vector2(5, 400), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "P", new Vector2(140, 400), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "More Particles:", new Vector2(5, 425), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "[", new Vector2(140, 425), draw.PropertyTextColor);

            draw.TextWriter.DrawString(draw.Font, "Rebuild Ball:", new Vector2(5, 450), draw.PropertyTextColor);
            draw.TextWriter.DrawString(draw.Font, "M", new Vector2(120, 450), draw.PropertyTextColor);
	    }

	    public void ProcessInput()
	    {
            if (KeyboardManager.KeyIsDown(Keys.X, 0.02f))
            {
                this.IncreaseRadius();
            }

            if (KeyboardManager.KeyIsDown(Keys.C, 0.02f))
            {
                this.DecreaseRadius();
            }

            float fBallRotationScale = MathHelper.Pi / 18.0f;

            if (KeyboardManager.KeyIsDown(Keys.V))
            {
                // Check if the Emitter is being rotated
                if (KeyboardManager.KeyWasJustPressed(Keys.J))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Down * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.L))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Up * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.I))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Left * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.K))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Right * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.U))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Backward * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.O))
                {
                    this.Emitter.OrientationData.RotationalVelocity += Vector3.Forward * fBallRotationScale;
                }
            }

            if (KeyboardManager.KeyIsDown(Keys.B))
            {
                // Check if the Emitter is being rotated
                if (KeyboardManager.KeyWasJustPressed(Keys.J))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Down * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.L))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Up * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.I))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Left * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.K))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Right * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.U))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Backward * fBallRotationScale;
                }

                if (KeyboardManager.KeyWasJustPressed(Keys.O))
                {
                    this.Emitter.OrientationData.RotationalAcceleration += Vector3.Forward * fBallRotationScale;
                }
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.N))
            {
                this.Emitter.OrientationData.RotationalVelocity = Vector3.Zero;
                this.Emitter.OrientationData.RotationalAcceleration = Vector3.Zero;
            }

            if (KeyboardManager.KeyIsDown(Keys.OemOpenBrackets, 0.04f))
            {
                this.MoreParticles();
            }

            if (KeyboardManager.KeyIsDown(Keys.P, 0.04f))
            {
                this.LessParticles();
            }

            if (KeyboardManager.KeyWasJustPressed(Keys.M))
            {
                this.RemoveAllParticles();
            }
	    }
	}
}