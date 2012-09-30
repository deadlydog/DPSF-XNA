<?php include "_HTMLPreHead.php"; // Includes the HTML version to use ?>
<title>DPSF Change Log (Dynamic Particle System Framework)</title>
<meta name="description" content="DPSF (Dynamic Particle System Framework) Change Log. View the changes made to DPSF from version to version." />
<meta name="keywords" content="DPSF, Dynamic, Particle, Particle System, Particle Systems, Framework, XNA, C#, Custom, Flexible, API, Library, Tool, Free, Flexible, Engine, Particle Engine, Particle Engines, Effect, Particle Effect, Particle Effects, Code, Change, Log, Change Log, Version, History" />
<?php include "_HTMLHead.php"; // Includes the Head information ?>
</head>

<body>
<?php include '_CommonSiteElements.php'; ?>

<div id="Content">
<h1>Change Log</h1>

<h2>XNA 4 Versions of DPSF</h2>

<h3>Version 2.3.0 - September 30, 2012</h3>
<ul>
	<li>Fixed bug in Sprite Particle System Templates and Sprite particle systems where they wouldn't use the SpriteBatch passed into the AutoInitialize() function.</li>
	<li>Added new ExplosionDebrisSpriteParticleSystem class, that shows the implementation differences between a 3D and 2D Sprite Particle System.</li>
	<li>Exposed the SpriteBatch object on the particle systems so that it can be retrieved.</li>
	<li>Added TotalNumberOfActiveParticles, TotalNumberOfParticlesAllocatedInMemory, and TotalNumberOfParticlesBeingDrawn virtual properties that can be overridden to return back more accurate numbers for particle systems that contain other particles systems.</li>
	<li>Refactored the DPSF Demo particle systems into wrapper classes that implement the new IWrapDPSFDemoParticleSystems interface to make it easier to add new DPSF Demo particle systems and clean up the GameMain.cs file so it is easier to understand.</li>
	<li>Refactored the DPSF Demo a lot, breaking the 5000+ line file into several smaller files and creating more reusable components (e.g. input managers, camera, DemoBase.cs). Created stand</li><li>alone BasicVirtualEnvironment project that houses all of these reusable components.</li>
	<li>Moved functionality to Linearly Interpolate (Lerp) the Emitter's position and orientation from the Update() function into new AddParticles() functions, so users can now have their particle's Lerp'd when manually adding particles without the emitter.</li>
	<li>Changed DPSF to throw custom DPSF Exceptions to make them easy to distinguish from other exceptions thrown by a users application.</li>
	<li>Added an Emitters collection to the particle systems, and the Emitter variable now always simply points to one of the emitters in the collection. This allows for particle systems to easily use multiple emitters.</li>
	<li>Added new MultipleEmittersParticleSystem particle system class that shows how to use multiple emitters in a particle system class.</li>
	<li>Added a NumberOfParticlesEmitted property to the Emitter class that keeps track of how many particles an individual emitter has added to a particle system.</li>
	<li>Updated documentation on some of the old tutorials and Optimizations section a bit.</li>
	<li>Added initial support for "Mono for Android" with a new DLL file (still working some bugs out though).</li>
	<li>Fixed bugs to make particle system properly serialize and deserialize.</li>
	<li>Added feature to skip the splash screen (by default) when debugging so developers don't have to see it every time they run their app.</li>
</ul>

<p>Possible Breaking Changes (see Updating Your Existing Particle Systems To Newer Versions of DPSF - 2.2.2 to 2.3.0 section of <a href="DPSFHelp/index.html">DPSF Help</a> for solutions):</p>
<ul>
	<li>Removed the mcRandomNumber particle system class variable. Everything should now just use the RandomNumber public property.</li>
	<li>Changed namespace of particle systems in the DPSF Demo from DPSF.ParticleSystems to DPSF_Demo.ParticleSystems.</li>
	<li>Moved the LerpEmittersPositionAndOrientation and LerpEmittersPositionAndOrientationOnNextUpdate properties from the particle system into the ParticleEmitter class, so that this can be configured for each emitter individually.</li>
	<li>Changed the MagnetList property from a LinkedList<DefaultParticleSystemMagnet> to a List<DefaultParticleSystemMagnet>.</li>
	<li>Changed IDPSFParticleVertex to implement Microsoft.XNA.Framework.Graphics.IVertexType and removed the old IDPSFParticleVertex enforcements.</li>
	<li>Changed the DPSF Splash Screen particle system class to be in the DPSF.SplashScreen namespace.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v2.2.2.exe">Version 2.2.2</a> - December 10, 2011</h3>
<ul>
	<li>Changed EULA to make using the DPSF Splash Screen optional. Now as long as DPSF is mentioned somewhere in the application (not necessarily through a splash screen) that is acceptable.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v2.2.1.exe">Version 2.2.1</a> - September 1, 2011</h3>
<ul>
	<li>Fixed bug with accepting input during the splash screen which resulted in a null ref exception.</li>
	<li>Now allow user input to skip the splash screen in the Windows Phone DPSF Demo.</li>
	<li>Fixed the DPSF Demo feature to not clear the screen every frame (F5).</li>
	<li>Fixed the DPSF Demo feature to draw static particles (F6).</li>
	<li>Fixed the DPSF Demo feature to create animated gifs and tilesets from particle systems (F8).</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v2.2.0.exe">Version 2.2.0</a> - July 1, 2011</h3>
<ul>
	<li>Changed all particle systems to use the built-in XNA 4 effects by default, rather than the DPSFDefaultEffect, so they are all now Reach compatible (i.e. Windows Phone 7 compatible).</li>
	<li>Substantial improvement to the performance of Quad particle systems, so it is very comparable with the Sprite3DBillboard particle systems now.</li>
	<li>Changed default Sampler State to LinearClamp, as PointClamp was causing aliasing problems with new effect types.</li>
	<li>Updated the DPSF Splash Screen to make it Reach compatible.</li>
	<li>Fixed bug in Particle System Manager where exception would be thrown by the manager if a particle system removed itself from the manager from within its Update() function.</li>
	<li>Fixed bug where Emitters would still emit particles when their Enabled property was set to false.</li>
	<li>Since we now use the built-in XNA 4 effects by default, added a ColorAsPremultiplied property to the particle base class, since the built-in effects expect premultiplied colors.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v2.1.0.exe">Version 2.1.0</a> - April 11, 2011</h3>
<ul>
	<li>Made DPSF compatible with the Reach profile configuration.</li>
    <li>Added new Sprite 3D Billboard particle system type which can be up to 50% faster than using billboarded Textured Quads.</li>
    <li>Converted most particle systems over to use the new Sprite 3D Billboard particle systems.</li>
    <li>Changed particle system's IsInitialized() function into IsInitialized boolean property.</li>
    <li>Fixed bug where particles wouldn't linearly interpolate (Lerp) between the emitter's previous and current position and orientation when it was moved manually (i.e. not with it's own acceleration/velocity properties).</li>
    <li>Added particle system properties LerpEmittersPositionAndOrientation and LerpEmittersPositionAndOrientationOnNextUpdate to control emitter Lerping.</li>
    <li>Added static DPSFDefaultSettings class to be used to set the default values to use for all DPSF particle systems when they are initialized.</li>
    <li>Added CopyFrom() function and Copy constructor to Emitter and AutoMemoryManagerSettings classes.</li>
    <li>Fixed bug in ParticleSystemManager's CopyFrom() function.</li>
    <li>Added PerformanceProfiling variables to the DPSF particle system class and Particle System Manager class that allow you to see how long it takes to perform an Update() or Draw() function call.</li>
    <li>Added new Trail, FireSprite, and MultipleParticleImagesSprite particle systems, and added templates for the Sprite 3D Billboard particle systems as well.</li>
    <li>Changed default SpriteSortMode from FrontToBack to Deferred.</li>
    <li>Fixed bug to treat the SpriteSortMode.Immediate mode like the other modes, since XNA 4 now supports using effects with any of the Sprite Sort Modes.</li>
    <li>Added FPSUpdated event to the static FPS class that fires every second after the FPS has been updated.</li>
    <li>Added ability to view performance stats in the DPSF Demo by pressing the F10 key.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v2.0.1.exe">Version 2.0.1</a> - February 14, 2011</h3>
<ul>
	<li>Fixed bug causing incompatible render settings run-time errors to be thrown.</li>
	<li>Added logic to more properly handle drawing too many particles on the Xbox 360, instead of throwing an exception.</li>
	<li>Fixed bug in DPSF Demo where it was still checking to make sure XNA 3.1 was installed and throwing an error if it wasn't.</li>
	<li>Gave assemblies a strong name.</li>
	<li>Added performance optimizations.</li>
	<li>Made DSPFDefaultEffect strongly typed, rather than just a generic Effect.</li>
	<li>Added virtual function SetCameraPosition() that may be overridden when not inheriting from the default DPSF particle system classes and will still work with the ParticleSystemManager.SetCameraPositionForAllParticleSystems() function.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v2.0.0.0.exe">Version 2.0.0.0</a> - January 11, 2011</h3>
<ul>
	<li>Made DPSF compatible with XNA GameStudio 4.0, including the Windows Phone 7 (using a 2D Sprite particle system only for now).</li>
	<li>Removed Pixel and Point Sprite particle system types, as they are no longer supported in XNA 4.0. You will need to convert any existing PointSpriteParticleSystem classes to be TexturedQuadParticleSystem classes, and add the EverytimeEvent "UpdateParticleToFaceTheCamera" to the particle system.</li>
	<li>Added WorldViewProjection property to return the result of multiplying the World, View, and Projection matrices.</li>
	<li>Sprite particle systems no longer default to using the custom DPSF effect (this allows it to run on the Windows Phone 7 without any changes). The custom DPSF effect can still be applied though by calling the function "SetEffectAndTechnique(DPSFDefaultEffect, DPSFDefaultEffectTechniques.Sprites.ToString());".</li>
	<li>Removed the InitializeRenderState() and ResetRenderState() virtual functions and replaced them with an InitializeRenderProperties() virtual function.</li>
	<li>Added DistanceFromCameraSquared property to all of the Quad particles.</li>
	<li>Added ParticleEvent "UpdateParticleDistanceFromCameraSquared" and ParticleSystemEvent "UpdateParticleSystemToSortParticlesByDistanceFromCamera" to sort particles based on their distance from the camera. These must be used if drawing textures with opaque and semi-transparent portions, where you don't want further away particles to be drawn overtop of closer particles.</li>
	<li>Wrapped all [Serializable] attributes in "if (WINDOWS)" conditional compilation statements, as binary serialization is not supported on the Xbox or Windows Phone.</li>
</ul>

<!-- XNA 3.1 Versions' Info Goes Here -->
<h2>XNA 3.1 Versions of DPSF</h2>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.6.0.exe">Version 1.6.0</a> - April 11, 2011</h3>
<ul>
	<li>Changed particle system's IsInitialized() function into IsInitialized property.</li>
    <li>Fixed bug where particles wouldn't linearly interpolate (Lerp) between the emitter's previous and current position and orientation when it was moved manually (i.e. not with it's own acceleration/velocity properties).</li>
    <li>Added particle system properties LerpEmittersPositionAndOrientation and LerpEmittersPositionAndOrientationOnNextUpdate to control emitter Lerping.</li>
    <li>Added static DPSFDefaultSettings class to be used to set the default values to use for all DPSF particle systems when they are initialized.</li>
    <li>Added CopyFrom() function and Copy constructor to Emitter and AutoMemoryManagerSettings classes.</li>
    <li>Fixed bug in ParticleSystemManager's CopyFrom() function.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.5.5.exe">Version 1.5.5</a> - February 14, 2011</h3>
<ul>
	<li>Added logic to more properly handle drawing too many particles on the Xbox 360, instead of throwing an exception.</li>
	<li>Added performance optimizations.</li>
	<li>Gave assemblies a strong name.</li>
	<li>Added virtual function SetCameraPosition() that may be overridden when not inheriting from the default DPSF particle system classes and will still work with the ParticleSystemManager.SetCameraPositionForAllParticleSystems() function.</li>
	<li>Updated help documentation.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.5.4.0.exe">Version 1.5.4.0</a> - December 8, 2010</h3>
<ul>
	<li>Changed install directory to be "DPSF (XNA 3.1)", rather than "DPSF" so that XNA 4 DPSF version can install to "DPSF".</li>
	<li>Fixed minor memory leak in the Particle System Manager when removing particle systems.</li>
	<li>SpriteBatchSettings now throws exception if trying to access it when not using a Sprite particle system.</li>
	<li>Added SpriteBatch parameter to the InitializeSpriteParticleSystem() functions to allow users to draw particle systems using their own SpriteBatches, instead of every particle system using its own. Also included a new UsingExternalSpriteBatchToDrawParticles property that tells at run-time if the particle system uses its own SpriteBatch or not.</li>
	<li>Added SpriteBatch parameter to the AutoInitialize() function to easily pass in an external Sprite Batch to use to draw Sprite particle systems.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.5.3.0.exe">Version 1.5.3.0</a> - November 10, 2010</h3>
<ul>
	<li>Fixed bug in Quad and Textured Quad particle systems where the texture was being drawn in reverse (upside down and mirrored).</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.5.2.0.exe">Version 1.5.2.0</a> - October 1, 2010</h3>
<ul>
	<li>Fixed enormous amount of garbage being created when using a Sprite type particle system, causing the garbage collector to run very often.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.5.1.0.exe">Version 1.5.1.0</a> - August 27, 2010</h3>
<ul>
	<li>Added SourceToTargetVector() and SourceToTargetDirection() functions to DPSFHelper.</li>
	<li>Added SetCameraPositionForAllParticleSystems() function to the Particle System Manager class.</li>
	<li>Reduced memory footprint of the Draw() function.</li>
	<li>Fixed exceptions that would be thrown when a particle system Destroyed itself.</li>
	<li>Added Particle Update functions to the Quad particle systems to constrain a particle around a specific axis (i.e. X, Y, or Z).</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.5.0.0.exe">Version 1.5.0.0</a> - April 25, 2010</h3>
<ul>
	 <li>Added check to make sure .NET Famework 3.5 or greater and XNA 3.1 are installed on the client's machine before running the DPSF Demo, and now displays a nice warning message if they are not installed.</li>
	<li>Catch and throw new exception when calling SetTexture() in the case that the texture you are loading has already been Dispose()'d.</li>
	<li>Fixed bug in Default Quad particle systems where particles with a size less than 2 would not be displayed.</li>
	<li>Now throw descriptive exceptions when properties try to be used before the particle system has been initialized.</li>
	<li>Added Size, StartSize, and EndSize properties to the Default Sprite and Quad Particles, which simply set the Width and Height properties.</li>
	<li>Updated help documentation a bit.</li>
	<li>Plastered the [Serializable] attribute everywhere to make DPSF serializable.</li>
	<li>Now throw more descriptive exceptions from the Particle System Manager class.</li>
	<li>Added DPSFDefaultEffectTechniques enumeration which specifies all of the available Techniques for the DPSFDefaultEffect.</li>
	<li>Added extra functions to the default particles.</li>
	<li>Fixed bug in ParticleSystemManager.RemoveAllParticleSystems() function that would cause an invalid index exception to be thrown.</li>
	<li>Fixed bug in ParticleSystemManager.DrawAllParticleSystems() function to draw particle systems in the correct order.</li>
	<li>Made all Particle and ParticleSystem Update Functions protected instead of public to promote particle system encapsulation and removed extra exposed functions that should not be used from users.</li>
	<li>Added explosion particle systems to the DPSF Demo.</li>
	<li>Included the DPSFDefaults.cs file used by DPSF for others to modify if wanted and to learn from.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.4.0.0.exe">Version 1.4.0.0</a> - December 4, 2009</h3>
<ul>
	<li>Added a FPSLogger class to the FPS.cs file included in the DPSF Demo project. This allows the FPS to be logged to a file.</li>
	<li>Added overloaded Initialize...ParticleSystem() functions to allow a Texture2D to be specified instead of a texture asset name string.</li>
	<li>Changed the Magnets class, adding MagnetPoint, MagnetLine, MagnetLineSegment, and MagnetPlane classes.</li>
	<li>Added DefaultSpriteTextureCoordinatesParticleSystem and DefaultTexturedQuadTextureCoordinatesParticleSystem classes for when texture 
	coordinates are required (not for animated particles), as well as the associated Particle classes and Particle Vertex structs.</li>
	<li>Added DefaultPointSpriteTextureCoordinatesParticleSystem, DefaultPointSpriteTextureCoordinatesNoRotationParticleSystem, and
	DefaultPointSpriteTextureCoordinatesNoColorParticleSystem classes, as well as associated Particle classes, Particle Vertex
	structs, and Effect file techniques.  Three classes are provided because the the NoRotation and NoColor classes run considerably faster than the DefaultPointSpriteTextureCoordinatesParticleSystem, as extra operations must be performed in the shaders to support both Color and Rotation when also using Texture Coordinates for Point Sprites. So if Texture Coordinates are required, but not both Color (including transparency) and Rotation, it is better to use the corresponding NoRotation or NoColor particle system classes.</li>
	<li>Added DefaultAnimatedPointSpriteParticleSystem, DefaultAnimatedPointSpriteNoRotationParticleSystem, and
	DefaultAnimatedPointSpriteNoColorParticleSystem classes, as well as assocatiated Particle classes and Particle Vertex structs.</li>
	<li>Added MultipleParticleImagePS particle system to DPSF demo to show how one particle system can be used to display many different particle images.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.3.1.0.exe">Version 1.3.1.0</a> - June 15, 2009</h3>
<ul>
	<li>Updated to be compatible with XNA Game Studio 3.1</li>
	<li>Added inner exception output to exceptions thrown when initializing particle systems.</li>
	<li>Changed the SherePS particle system to use the Particle texture.</li>
	<li>Fixed formula for calculating the row, column, and layer a particle is at in the Grid particle systems.</li>
	<li>Now throws an exception when trying to Draw on the XBox 360 with a vertex buffer that is too large.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.3.0.0.exe">Version 1.3.0.0</a> - May 1, 2009</h3>
<ul>
    <li>Fixed bug in DPSFHelper.RandomNormalizedVector() function where it only returned positive vectors.</li>
    <li>Renamed Emitter.EmitParticles property to Emitter.EmitParticlesAutomatically.</li>
    <li>Added Emitter.Enabled property.</li>
    <li>Added Emitter.BurstParticles and Emitter.BurstTime properties to release particles in bursts.</li>
    <li>Renamed UpdateParticleSystemTurnEmitterOn/Off() functions to UpdateParticleSystemEmitParticlesAutomaticallyOn/Off().</li>
    <li>Added UpdateParticleSystemEnableEmitter() and UpdateParticleSystemDisableEmitter() particle system update functions to the Default classes.</li>
    <li>Fixed bug where an "index out of range" exception would be thrown when removing Events from within an Event function.</li>
    <li>Renamed MultipleChangeParticleSystem.cs to SquarePatternParticleSystem.cs and changed some of its functions.</li>
    <li>Formatted a lot of the XML documentation better.</li>
    <li>Fixed bug in DPSFDefaultEffect.fx which caused Point Sprites to be displayed smaller when their rotation was zero.</li>
    <li>Created first 5 tutorials in the Help Documentation.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.2.0.0.exe">Version 1.2.0.0</a> - March 23, 2009</h3>
<ul>
    <li>Changed what happens a little bit when a particle is in the exact same position as a magnet affecting it, so now the particle is only pushed in a random direction if the magnet is repelling it (not attracting it).</li>
	<li>Renamed the DefaultParticleSystemMagnet.DistanceFunctions enumeration's value "SameForceAtAllDistances" to "Constant".</li>
	<li>Added Grid particle system to Demo.</li>
	<li>Added Sphere particle system to Demo.</li>
	<li>Fixed bug where exception would be thrown when reducing the NumberOfParticlesAllocatedInMemory to less than what was currently in the particle system already.</li>
	<li>Added global variable miPARTICLE_SYSTEM_UPDATES_PER_SECOND to GameMain.cs to control how often the particle systems are updated.</li>
	<li>Fixed bug in the ParticleSystemManager.AutoInitializeAllParticleSystems() function where not all Particle Systems were being initialized.</li>
	<li>Added DPSFHelper.RandomNumberBetween(fValue1, fValue2) function for returning random numbers in a given range.</li>
	<li>Fixed bug to in the Default Point Sprite and Quad Particle System classes and Templates to reset the RenderState's AlphaBlendEnable and AlphaTestEnable properties to false after Draw() is called.</li>
	<li>Added SimulationSpeed and UpdatesPerSecond properties to the individual particle systems, and changing the Particle System Manager's SimulationSpeed / UpdatesPerSecond now sets the individual particle systems' properties.</li>
	<li>Changed the SimulationSpeed and UpdatesPerSecond properties to work the same when inheriting from DrawableGameComponent or not.</li>
	<li>Added ParticleSystemManagerToCopyPropertiesFrom property to the particle systems (Copies SimulationSpeed and UpdatesPerSecond).</li>
	<li>Added a NoDisplay Particle Type that can offer better performance for particle systems that do not need to be visualized (i.e. drawn to the screen), such as for some research applications or when using a DPSF Particle System as an internal data structure.</li>
	<li>Added a RandomNumbers class that inherits the Random class and adds more functionality.</li>
	<li>Replaced the PointOnVertical/HorizontalCircle() functions in DPSFHelper with more generic PointOnSphere() functions.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.1.1.0.exe">Version 1.1.1.0</a> - Feb 24, 2009</h3>
<ul>
    <li>Fixed bug where a particle would be given an invalid position when it was in the exact same position as a magnet affecting it.</li>
	<li>Added global boolean values to GameMain.cs to turn building an Animated GIF and a Tile Set Image on/off when building images from an animation.</li>
</ul>

<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.1.0.0.exe">Version 1.1.0.0</a> - Feb 11, 2009</h3>
<ul>
    <li>Added the SpriteBatchSettings and AutoMemoryManagerSettings properties to the IDPSFParticleSystem interface so they are more accessible.</li>
    <li>Added the ability to set all of the Sprite particle systems' Transformation Matrix property from the Particle System Manager.</li>
    <li>When multiple particles are added within the same frame, their Elapsed Time's are linearly interpolated so that it appears that the particles were added smoothly between frames.</li>
    <li>Added Sparkler particle system.</li>
    <li>Changed default installation path from C:\Program Files\DPSF to C:\DPSF.</li>
</ul>
<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.0.1.1.exe">Version 1.0.1.1</a> - Feb 9, 2009</h3>
<ul>
    <li>Renamed the DPSFParticle classes function name from UpdateAutomaticVariables() to UpdateElapsedTimeVariables().</li>
    <li>Renamed the default vertex update function from UpdateParticleVertex() to UpdateVertexProperties().</li>
    <li>Renamed the DPSFParticle classes virtual function name from CopyFromParticle() to CopyFrom().</li>
    <li>Updated DPSF Demo to place the text in a different position so that it is visible on the Xbox 360.</li>
</ul>
<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.0.1.0.exe">Version 1.0.1.0</a> - Feb 5, 2009</h3>
<ul>
    <li>Fixed display issues with Sprites and Textured Quads on the Xbox 360 when using the DPSF Default Effect.</li>
    <li>Removed OldPosition and OldOrientation variables from the ParticleEmitter class, as they were unnecessary.</li>
    <li>Updated the DPSF Demo and installer to include a .sln file, which can be run on Windows and the Xbox 360.</li>
</ul>
<h3><a href="ArchivedDPSFVersions/DPSF Installer v1.0.0.0.exe">Version 1.0.0.0</a> - Feb 2, 2009</h3>
<ul>
    <li>This is the first official release of DPSF.</li>
</ul>

</div>

<?php include '_Footer.php'; ?>
</body>
</html>
